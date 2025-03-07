using System.Security.Claims;
using System.Security.Principal;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisssStock.Application.DTOs;
using VisssStock.Infrastructure.HTTP.SSO.JWT;
using VisssStock.Application.Models;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using VisssStock.Application.Interfaces;

namespace VisssStock.Application.Services.AuthService
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtTokenProvider _jwtTokenProvider;

        private readonly IConfiguration _configuration;

        public AuthServiceImpl(DataContext context, IMapper mapper, IJwtTokenProvider jwtTokenProvider,
        IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _jwtTokenProvider = jwtTokenProvider;
            _configuration = configuration;
        }

        public AuthServiceImpl()
        {
        }

        public async Task<ServiceResponse<UserDTO>> createSingleUser(CreateUserDTO createUserDto)
        {
            var serviceResponse = new ServiceResponse<UserDTO>();
            if (_context.Users.FirstOrDefault(x => x.UserName.ToLower() == createUserDto.UserName.Trim().ToLower()) != null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Tài khoản đã tồn tại !";
                return serviceResponse;
            }

            // check role có tồn tài 
            foreach (var role in createUserDto.Roles)
            {
                var roleCheck = _context.Roles.FirstOrDefault(r => r.Id == role.RoleId);
                if (roleCheck == null)
                {
                    serviceResponse.Status = false;
                    serviceResponse.ErrorCode = 400;
                    serviceResponse.Message = "Không tìm thấy vai trò này !";
                    return serviceResponse;
                }

                if (roleCheck.Name == _configuration.GetSection("Role:HOCSINH").Value)
                {
                    if (role.SubjectId.Count > 0)
                    {
                        serviceResponse.Status = false;
                        serviceResponse.ErrorCode = 400;
                        serviceResponse.Message = "Không thể gán môn học cho học sinh !";
                        return serviceResponse;
                    }
                }
                 
            }

            // tạo tài khoản
            createUserDto.Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            User userRegister = _mapper.Map<User>(createUserDto);
            var userSaved = _context.Users.Add(userRegister);
            await _context.SaveChangesAsync();

            var listUR = new List<UserRole>(); 

            foreach (var roleRegisterUser in createUserDto.Roles)
            {
                //if (roleRegisterUser.SubjectId.Count > 0)
                //{
                //    //if (roleRegisterUser.GradesID.Count > 0)
                //    //{
                //    //    foreach (int gradeid in roleRegisterUser.GradesID)
                //    //    {
                //    //        var ur = new UserRole();
                //    //        ur.UserId = userSaved.Entity.Id;
                //    //        ur.RoleId = roleRegisterUser.RoleId;
                //    //        ur.SubjectId = roleRegisterUser.SubjectId[0]; 
                //    //        ur.GradeID = gradeid;
                //    //        var userole = _context.UserRole.Where(ur => ur.UserId == userSaved.Entity.Id && ur.RoleId == roleRegisterUser.RoleId
                //    //                                                            && ur.SubjectId == roleRegisterUser.SubjectId[0] && ur.GradeID == gradeid).FirstOrDefault();
                //    //        if (userole == null)
                //    //        {
                //    //            listUR.Add(ur);
                //    //        }
                //    //    }
                //    //}
                //    //else
                //    //{
                //        var userole = _context.UserRoles.Where(u => u.UserId == userSaved.Entity.Id && u.RoleId == roleRegisterUser.RoleId).FirstOrDefault();
                //        if (userole == null)
                //        { 
                //            var ur = new UserRole();
                //            ur.UserId = userSaved.Entity.Id;
                //            ur.RoleId = roleRegisterUser.RoleId;
                //            //ur.SubjectId = roleRegisterUser.SubjectId[0];
                //            listUR.Add(ur);
                //        }
                //    //}
                //}
                //else
                //{
                    var userole = _context.UserRoles.Where(ur => ur.UserId == userSaved.Entity.Id && ur.RoleId == roleRegisterUser.RoleId).FirstOrDefault();
                    if (userole == null)
                    { 
                        var ur = new UserRole();
                        ur.UserId = userSaved.Entity.Id;
                        ur.RoleId = roleRegisterUser.RoleId;
                        listUR.Add(ur);
                    }
                //}
            }

            //save Role 
            listUR.ForEach(x => _context.UserRoles.Add(x));

            //Save Function
            

            await _context.SaveChangesAsync(); 
            //var folder = new Folder();
            //folder.Name = "Thư mục của " + createUserDto.LastName + " " + createUserDto.FirstName;
            //folder.CreateBy = userSaved.Entity.Id;
            //folder.CreateDate = DateTime.Now;
            //folder.ParentId = 0;
            //_context.Folder.Add(folder);
            //await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<UserDTO>(userRegister);

            return serviceResponse;
        }


        public async Task<LoginResponse> login(LoginFrom loginFrom)
        {
            var loginResponse = new LoginResponse();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(loginFrom.username));

            if (user == null)
            {
                loginResponse.ErrorCode = 400;
                loginResponse.Status = false;
                loginResponse.Message = "Tài khoản hoặc mật khẩu không chính xác!";
                return loginResponse;
            }

            if (user.Enable == 1)
            {
                loginResponse.ErrorCode = 400;
                loginResponse.Status = false;
                loginResponse.Message = "Tài khoản đã bị vô hiệu hóa!";
                return loginResponse;
            }

            if (!VerifyPassword(loginFrom.password, user.Password))
            {
                loginResponse.ErrorCode = 400;
                loginResponse.Status = false;
                loginResponse.Message = "Tài khoản hoặc mật khẩu không chính xác!";
                return loginResponse;
            }

            IPrincipal userLogin = await createPrincipal(user);
            var token = _jwtTokenProvider.CreateToken(userLogin);

            List<Role> roles = getRolesByUserId(user.Id);
            loginResponse.role = roles;
            loginResponse.accessToken = token;
            // loginResponse.user = user;
             
            return loginResponse;
        }

        public async Task<ServiceResponse<UploadExcelFileResponse>> createMultiUser(
        UploadExcelFileRequest uploadExcelFileRequest)
        {
            return null;
        }

        public async Task<ServiceResponse<ResetPasswordForm>> resetPassword(ResetPasswordForm resetPasswordForm,
        int userId)
        {
            var serviceResponse = new ServiceResponse<ResetPasswordForm>();
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (VerifyPassword(resetPasswordForm.currentPassword, user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordForm.newPassword);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Mật khẩu không chính xác !";
                return serviceResponse;
            }

            serviceResponse.Message = "OK";
             

            return serviceResponse;
        }

        public async Task<ServiceResponse<IActionResult>> resetPasswordAdmin(int id)
        {
            var serviceResponse = new ServiceResponse<IActionResult>();
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            user.Password = BCrypt.Net.BCrypt.HashPassword("12345678");
            _context.Users.Update(user); 
            await _context.SaveChangesAsync();
            string smtpEmail = _configuration.GetSection("SMTP_ACCOUNT:SMTP_USER").Value;
            string smtpPassword = _configuration.GetSection("SMTP_ACCOUNT:SMTP_PASS").Value;
            string content = @"
                                    <b>Chào " + user.FirstName + " " + user.LastName + @",</b> <br />

                                    Chúng tôi xin gửi đến bạn thông tin về mật khẩu mới cho tài khoản của bạn. <br />Đây là mật khẩu tạm thời, bạn nên đổi mật khẩu ngay sau khi đăng nhập để đảm bảo tính bảo mật cho tài khoản của mình. <br />

                                    Mật khẩu mới của bạn là: <b>12345678</b> <br />

                                    Chúng tôi khuyên bạn không nên chia sẻ mật khẩu này với bất kỳ ai khác và luôn luôn bảo mật thông tin cá nhân của mình. <br />
                                    Chân thành cảm ơn bạn đã sử dụng dịch vụ. <br /><br />

                                    Trân trọng, <br />
                                    <b>VICTORIA THĂNG LONG</b>";
            //string message = Utilities.SendEmail(smtpEmail, smtpPassword, "Thông báo mật khẩu đến tài khoản " + user.UserName, content, user.Email);
            //serviceResponse.Message += message;
            return serviceResponse;
        }

        public async Task<ServiceResponse<UpdateProfileDTO>> updateProfile(UpdateProfileDTO updateProfile, int userId)
        {
            var serviceResponse = new ServiceResponse<UpdateProfileDTO>();
            var dbUsers = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
            if (dbUsers == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy người dùng này !";
                return serviceResponse;
            }

            dbUsers.FirstName = updateProfile.FirstName;
            dbUsers.LastName = updateProfile.LastName;
            dbUsers.Gender = updateProfile.Gender;
            dbUsers.BirthDate = updateProfile.BirthDate;
            dbUsers.Address = updateProfile.Address;
            dbUsers.Phone = updateProfile.Phone;
            _context.Users.Update(dbUsers);
            await _context.SaveChangesAsync();
            serviceResponse.Data = updateProfile;
             

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<MenuDTO>>> getMenuItems(int userId)
        {
            var serviceResponse = new ServiceResponse<List<MenuDTO>>();
            var _listPath = (from user in _context.Users
                             join userRole in _context.UserRoles on user.Id equals userRole.UserId
                             join role in _context.Roles on userRole.RoleId equals role.Id
                             join roleMenu in _context.RoleMenus on role.Id equals roleMenu.RoleId
                             join menu in _context.Menus on roleMenu.MenuId equals menu.Id
                             where user.Id == userId && menu.IsDeleted == 0
                             select menu
            ).Distinct().OrderBy(x => x.Orderno).ToList();
            var listMenu = new List<MenuDTO>();
            var listSubHeader = _listPath.DistinctBy(x => x.Subheader).ToList();


            foreach (var sMenu in listSubHeader)
            {
                var menu = new MenuDTO();
                menu.Subheader = sMenu.Subheader;
                var listItem = new List<Items>();
                var _listMenuDb = _listPath.Where(x => x.ParentId == 0 && x.Subheader.Equals(sMenu.Subheader)).ToList();
                foreach (var _lmsh in _listMenuDb)
                {
                    var parentId = _lmsh.Id;
                    var listChil = _listPath.Where(x => x.ParentId == parentId).ToList();
                    var listChilDTO = listChil.Select(x => new ChildrenDTO(x.Title, x.Path)).ToList();
                    var _Item = new Items();
                    _Item.Title = _lmsh.Title;
                    _Item.Path = _lmsh.Path;
                    _Item.Icon = _lmsh.Icon;
                    _Item.Children = listChilDTO;
                    listItem.Add(_Item);
                }

                menu.Items = listItem;
                listMenu.Add(menu);
            }


            serviceResponse.Data = listMenu;
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<MenuDTO>>> getMenuItemsV2(int userId)
        {
            var serviceResponse = new ServiceResponse<List<MenuDTO>>();
            var _listPath = (from user in _context.Users
                             join userRole in _context.UserRoles on user.Id equals userRole.UserId
                             join role in _context.Roles on userRole.RoleId equals role.Id
                             join roleMenu in _context.RoleMenus on role.Id equals roleMenu.RoleId
                             join menu in _context.Menus on roleMenu.MenuId equals menu.Id
                             where user.Id == userId
                             select menu
            ).Distinct().OrderBy(x => x.Subheader).ToList();
            var listMenu = new List<MenuDTO>();
            var listSubHeader = _listPath.DistinctBy(x => x.Subheader).ToList();


            foreach (var sMenu in listSubHeader)
            {
                var menu = new MenuDTO();
                menu.Subheader = sMenu.Subheader;
                var listItem = new List<Items>();
                var _listMenuDb = _listPath.Where(x => x.ParentId == 0 && x.Subheader.Equals(sMenu.Subheader)).ToList();
                foreach (var _lmsh in _listMenuDb)
                {
                    var parentId = _lmsh.Id;
                    var listChil = _listPath.Where(x => x.ParentId == parentId).ToList();
                    var listChilDTO = listChil.Select(x => new ChildrenDTO(x.Title, x.Path)).ToList();
                    var _Item = new Items();
                    _Item.Title = _lmsh.Title;
                    _Item.Path = _lmsh.Path;
                    _Item.Icon = _lmsh.Icon;
                    _Item.Children = listChilDTO;
                    listItem.Add(_Item);
                }

                menu.Items = listItem;
                listMenu.Add(menu);
            }


            serviceResponse.Data = listMenu;
            return serviceResponse;
        }


        private async Task<IPrincipal> createPrincipal(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id + "")
            };
            var roles = getRolesByUserId(user.Id);
            var listPermission = new List<Permission>();
            foreach (var role in roles)
            {
                listPermission.AddRange(getPermissionByRoleId(role.Id));
            }

            claims.AddRange(listPermission.Select(p => new Claim(ClaimTypes.Role, p.Name)));
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }


        private bool VerifyPassword(string password, string storePassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, storePassword);
        }

        private List<Role> getRolesByUserId(int id)
        {
            var roles = new List<Role>();
            var _list = (from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where u.Id == id
                         select r).Distinct().ToList();
            foreach (var r in _list)
            {
                roles.Add(r);
            }

            return roles;
        }

        private List<Permission> getPermissionByRoleId(int roleId)
        {
            var permissions = new List<Permission>();
            var _list = (from r in _context.Roles
                         join rp in _context.RolePermissions on r.Id equals rp.RoleId
                         join p in _context.Permissions on rp.PermissionId equals p.Id
                         where r.Id == roleId
                         select new
                         {
                             id = p.Id,
                             name = p.Name,
                             description = p.Description
                         }).ToList();
            foreach (var p in _list)
            {
                var permission = new Permission();
                permission.Id = p.id;
                permission.Name = p.name;
                permission.Description = p.description;
                permissions.Add(permission);
            }

            return permissions;
        }
    }
}
