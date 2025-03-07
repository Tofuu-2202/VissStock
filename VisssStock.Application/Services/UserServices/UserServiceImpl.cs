using System.Data;
using System.Net;
using AutoMapper;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination; 
using VisssStock.Domain.DataObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ServiceStack;
using VisssStock.Infrastructure.Data;
using VisssStock.Application.Interfaces;


namespace VisssStock.Application.Services.UserServices
{
    public class UserServiceImpl : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserServiceImpl(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<UserDTO>> getRoles(int userid)
        {
            var userDTOs = new UserDTO();
            var serviceResponse = new ServiceResponse<UserDTO>();
            var dbUsers = await _context.Users.FirstOrDefaultAsync(c => c.Id == userid);
            if (dbUsers == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy người dùng này !";
                return serviceResponse;
            }

            userDTOs = _mapper.Map<UserDTO>(dbUsers);
            userDTOs.roles = getRolesByUserId(userid);
            serviceResponse.Data = userDTOs;
            return serviceResponse;
        }

        public async Task<ServiceResponse<PagedList<UserDTO>>> getAllUsers(OwnerParameters ownerParameters,
         string searchByName, int? roleId, string searchByEmail,
         int? userId)
        {
            var serviceResponse = new ServiceResponse<PagedList<UserDTO>>();
            var userDTOs = new List<UserDTO>();
            var dbUsers = await _context.Users.ToListAsync();
            if (roleId != null)
            {
                dbUsers = (from u in _context.Users
                           join userRole in _context.UserRoles on u.Id equals userRole.UserId
                           join role in _context.Roles on userRole.RoleId equals role.Id
                           where role.Id == roleId
                           select u
                ).Distinct().ToList();
            }

            if (userId != null)
            {
                dbUsers = dbUsers.Where(x => x.Id == userId).ToList();
            }

            if (!String.IsNullOrEmpty(searchByName))
            {
                dbUsers = dbUsers.Where(x =>
                x.FirstName.ToUpper().Contains(searchByName.ToUpper()) ||
                x.LastName.ToLower().Contains(searchByName.ToLower())).ToList();
            }

            if (!String.IsNullOrEmpty(searchByEmail))
            {
                dbUsers = dbUsers.Where(x => x.Email.ToUpper().Contains(searchByEmail.ToUpper())).ToList();
            }

            userDTOs = dbUsers.Select(u => _mapper.Map<UserDTO>(u)).ToList();
            foreach (var dbUserDTO in userDTOs)
            {
                dbUserDTO.roles = await getRolesByUserIdGetAll(dbUserDTO.Id);
            }

            serviceResponse.Data = PagedList<UserDTO>.ToPagedList(
            userDTOs.AsEnumerable<UserDTO>().OrderBy(on => on.Email),
            ownerParameters.pageIndex,
            ownerParameters.pageSize);
            return serviceResponse;


            //var serviceResponse = new ServiceResponse<PagedList<UserDTO>>();
            //var dbUsers = (from user in _context.User
            //               join userRole in _context.UserRole on user.Id equals userRole.UserId
            //               join role in _context.Role on userRole.RoleId equals role.Id
            //               where user.FirstName.ToLower().Contains(string.IsNullOrEmpty(searchByName) ? "" : searchByName.ToLower())
            //                     || user.LastName.ToLower().Contains(string.IsNullOrEmpty(searchByName) ? "" : searchByName.ToLower())
            //                     & user.Email.ToLower().Contains(string.IsNullOrEmpty(searchByEmail) ? "" : searchByEmail.ToLower())
            //                     & (role.Id == roleId || role.Id == null)
            //               select user
            //).Distinct().ToListAsync();
            //serviceResponse.Data = PagedList<UserDTO>.ToPagedList(
            //dbUsers.Result.Select(x => getUserDTO(x).Result).AsEnumerable<UserDTO>().OrderByDescending(on => on.Id),
            //ownerParameters.pageIndex,
            //ownerParameters.pageSize);
            //return serviceResponse;
        }

        public async Task<ServiceResponse<PagedList<UserDTO>>> getAllUserWithInfor(OwnerParameters ownerParameters,
        string searchByName, int? roleId, string searchByEmail,
        int? userId)
        {
            var serviceResponse = new ServiceResponse<PagedList<UserDTO>>();
            var userDTOs = new List<UserDTO>();
            var dbUsers = (from user in _context.Users
                           join userRole in _context.UserRoles on user.Id equals userRole.UserId
                           join role in _context.Roles on userRole.RoleId equals role.Id
                           where user.FirstName.ToLower().Contains(string.IsNullOrEmpty(searchByName) ? "" : searchByName.ToLower())
                                 || user.LastName.ToLower().Contains(string.IsNullOrEmpty(searchByName) ? "" : searchByName.ToLower())
                           where user.Email.ToLower().Contains(string.IsNullOrEmpty(searchByEmail) ? "" : searchByEmail.ToLower())
                           where role.Id == roleId || role.Id == null
                           where user.Id == userId || user.Id == null
                           select user
            ).Distinct().ToListAsync().Result.Select(x => getUserDTOInfor(x)).ToList();
            serviceResponse.Data = PagedList<UserDTO>.ToPagedList(
            userDTOs.AsEnumerable<UserDTO>().OrderByDescending(on => on.Id),
            ownerParameters.pageIndex,
            ownerParameters.pageSize);
            return serviceResponse;
        }


        public async Task<ServiceResponse<UserDTO>> getUserById(int id)
        {
            var serviceResponse = new ServiceResponse<UserDTO>();

            var dbUsers = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
            if (dbUsers == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy người dùng này !";
                return serviceResponse;
            }
            else
            {
                serviceResponse.Data = getUserDTOInfor(dbUsers).Result;
                serviceResponse.Status = true;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UpdateUserDTO>> updateUser(UpdateUserDTO userDto, int id)
        {
            var serviceResponse = new ServiceResponse<UpdateUserDTO>();
            var dbUsers = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
            if (dbUsers == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy người dùng này !";
                return serviceResponse;
            }
            else
            {
                foreach (var role in userDto.Roles)
                {
                    var roleCheck = await _context.Roles.FirstOrDefaultAsync(r => r.Id == role.RoleId);
                    if (roleCheck == null)
                    {
                        serviceResponse.Status = false;
                        serviceResponse.ErrorCode = 400;
                        serviceResponse.Message = "Không tìm thấy vai trò này !";
                        return serviceResponse;
                    }
                    else
                    {
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
                }

                var listUR = new List<UserRole>();
                foreach (var roleRegisterUser in userDto.Roles)
                {
                    //var checkNull = roleRegisterUser.SubjectId.Count > 0;
                    //if (checkNull)
                    //{
                    //    foreach (var s in roleRegisterUser.SubjectId)
                    //    {
                    //        var ur = new UserRole();
                    //        ur.UserId = id;
                    //        ur.RoleId = roleRegisterUser.RoleId; 
                    //        listUR.Add(ur);
                    //    }
                    //}
                    //else
                    //{
                        var ur = new UserRole();
                        ur.UserId = id;
                        ur.RoleId = roleRegisterUser.RoleId;
                        listUR.Add(ur);
                    //}
                }

                var listDB = await _context.UserRoles.Where(x => x.UserId == id).ToListAsync();
                foreach (var db in listDB)
                {
                    db.IsDeleted = 1;
                    _context.UserRoles.Update(db);
                    await _context.SaveChangesAsync();
                }

                foreach (var rq in listUR)
                {
                    _context.UserRoles.Add(rq);
                    await _context.SaveChangesAsync();
                }

                dbUsers.Email = userDto.Email;
                dbUsers.FirstName = userDto.FirstName;
                dbUsers.LastName = userDto.LastName;
                dbUsers.Gender = userDto.Gender;
                dbUsers.BirthDate = userDto.BirthDate;
                dbUsers.Address = userDto.Address;
                dbUsers.Phone = userDto.Phone;
                dbUsers.Enable = userDto.Enable;
                _context.Users.Update(dbUsers);
                await _context.SaveChangesAsync();
                serviceResponse.Data = userDto;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UserDTO>> editActiveUser(int id)
        {
            var serviceResponse = new ServiceResponse<UserDTO>();
            var dbUsers = await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
            if (dbUsers == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy người dùng nào !";
                return serviceResponse;
            }

            if (dbUsers.Enable == 0)
            {
                dbUsers.Enable = 1;
            }
            else
            {
                dbUsers.Enable = 0;
            }

            serviceResponse.Data = _mapper.Map<UserDTO>(dbUsers);
            await _context.SaveChangesAsync();
            return serviceResponse;
        }

        private List<RoleDTOUser> getRolesByUserId(int id)
        {
            var _listRoleDTO = new List<RoleDTOUser>();
            var _list = (from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where u.Id == id
                         select r).Distinct().ToList();

            foreach (var role in _list)
            {
                var _roleDTO = new RoleDTOUser();
                _roleDTO.Id = role.Id;
                _roleDTO.Name = role.Name;
                _roleDTO.Description = role.Description; 
                
                _listRoleDTO.Add(_roleDTO);
            }

            return _listRoleDTO;
        }

        private async Task<List<RoleDTOUser>> getRolesByUserIdGetAll(int id)
        {
            var _listRoleDTO = new List<RoleDTOUser>();
            var _list = (from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where u.Id == id
                         select r).Distinct().ToList();

            foreach (var role in _list)
            {
                var _roleDTO = new RoleDTOUser();
                _roleDTO.Id = role.Id;
                _roleDTO.Name = role.Name;
                _roleDTO.Description = role.Description;
                
                _listRoleDTO.Add(_roleDTO);
            }

            return _listRoleDTO;
        }
         
        private async Task<UserDTO> getUserDTOInfor(User user)
        {
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.roles = (from u in _context.Users
                             join ur in _context.UserRoles on u.Id equals ur.UserId
                             join r in _context.Roles on ur.RoleId equals r.Id
                             where u.Id == user.Id
                             select r).Distinct().ToListAsync().Result.Select(x => _mapper.Map<RoleDTOUser>(x)).ToList();

            return userDTO;
        }

         

         
         

        private async Task<UserDTO> getUserDTO(User user)
        {
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.roles = (from u in _context.Users
                             join ur in _context.UserRoles on u.Id equals ur.UserId
                             join r in _context.Roles on ur.RoleId equals r.Id
                             where u.Id == user.Id
                             select r).Distinct().ToListAsync().Result.Select(x => _mapper.Map<RoleDTOUser>(x)).ToList();
            return userDTO;
        }

        public async Task<ServiceResponse<User>> createUsers(User user, int roleId, string subjectId)
        {
            var serviceResponse = new ServiceResponse<User>();
            // check tài khoản đã taọ

            if (await _context.Users.FirstOrDefaultAsync(
                x => x.UserName == user.UserName || x.Email == user.Email) != null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Tài khoản đã tồn tại !";
                return serviceResponse;
            }
            else
            {
                // check role có tồn tài 
                if (await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId) == null)
                {
                    serviceResponse.Status = false;
                    serviceResponse.ErrorCode = 400;
                    serviceResponse.Message = "Không tìm thấy vai trò này !";
                    return serviceResponse;
                }
                else
                {
                     

                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    var saved = _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    var ur = new UserRole();
                    ur.UserId = saved.Entity.Id;
                    ur.RoleId = roleId;
                    
                    _context.UserRoles.Add(ur);
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = saved.Entity;
                }
            }

            return serviceResponse;
        }

    }
}
