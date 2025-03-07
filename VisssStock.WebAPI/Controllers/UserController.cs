using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.UserServices;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        IExcelDataReader reader;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, DataContext context, IConfiguration configuration)
        {
            _userService = userService;
            _context = context;
            _configuration = configuration;
        }

        // [HttpGet("getPermissions")]
        // public ActionResult getPermissions(int userid, int roleid)
        // {
        //     using (var ctx = new DataContext())
        //     {
        //         //Get student name of string type
        //         var permissions = ctx.Permission.FromSqlRaw(@"SELECT p.* FROM user u
        //                                     inner join user_role ur on u.ID = ur.userID
        //                                     inner join role r on ur.roleID = r.id
        //                                     inner join role_permission rp on r.id = rp.roleid
        //                                     inner join permission p on p.id = rp.permissionid
        //                                     where u.id = '{0}' and r.id = '{1}'", userid, roleid);
        //         return Ok(permissions);
        //     }
        // }
        //
        // [HttpGet("getRoles")]
        // public async Task<ActionResult<ServiceResponse<List<UserDTO>>>> getRoles(int userid)
        // {
        //     var response = await _userService.getRoles(userid);
        //
        //     if (response.Status == false)
        //     {
        //         return BadRequest(new ProblemDetails
        //         {
        //             Status = response.ErrorCode,
        //             Title = response.Message
        //         });
        //     }
        //
        //     return Ok(response);
        // }

        [HttpGet("getAllUserWithInfor")]
        public async Task<ActionResult<ServiceResponse<PagedList<UserDTO>>>> getAllUserWithInfor(
        [FromQuery] OwnerParameters ownerParameters, string? searchByName, int? roleId, string? searchByEmail,
        int? userId)
        {
            var userIdLogin = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userIdLogin
            where p.Name == _configuration.GetSection("Permission:USER_VIEW_ALL").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }

            var users = await _userService.getAllUserWithInfor(ownerParameters, searchByName, roleId, searchByEmail, userId);
            var metadata = new
            {
                users.Data.TotalCount,
                users.Data.PageSize,
                users.Data.CurrentPage,
                users.Data.TotalPages,
                users.Data.HasNext,
                users.Data.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(users);
        }
        [HttpGet("getAll")]
        public async Task<ActionResult<ServiceResponse<PagedList<UserDTO>>>> getAllUsers(
        [FromQuery] OwnerParameters ownerParameters, string? searchByName, int? roleId, string? searchByEmail,
        int? userId)
        {
            var userIdLogin = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userIdLogin
            where p.Name == _configuration.GetSection("Permission:USER_VIEW_ALL").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }

            var users = await _userService.getAllUsers(ownerParameters, searchByName, roleId, searchByEmail, userId);
            var metadata = new
            {
                users.Data.TotalCount,
                users.Data.PageSize,
                users.Data.CurrentPage,
                users.Data.TotalPages,
                users.Data.HasNext,
                users.Data.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(users);
        }

        [HttpGet("getOne/{id}")]
        public async Task<ActionResult<ServiceResponse<UserDTO>>> getOne(int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:USER_VIEW_DETAIL").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }

            var response = await _userService.getUserById(id);
            if (response.Status == false)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = response.ErrorCode,
                    Title = response.Message
                });
            }

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceResponse<UpdateUserDTO>>> updateUser(
        [FromBody] UpdateUserDTO userDto, int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:USER_UPDATE").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }

            var response = await _userService.updateUser(userDto, id);
            if (response.Status == false)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = response.ErrorCode,
                    Title = response.Message
                });
            }

            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<UserDTO>>> editActiveUser(int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:USER_EDIT_ACTIVE").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }

            var response = await _userService.editActiveUser(id);
            if (response.Status == false)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = response.ErrorCode,
                    Title = response.Message
                });
            }

            return Ok(response);
        }


        [HttpPost("UploadExcel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ServiceResponse<User>>> UploadExcel(IFormFile file)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:AUTH_CREATE_MULTI_USER").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }

            if (file == null)
                throw new System.Exception("File is Not Received...");
            // MAke sure that only Excel file is used 
            string dataFileName = Path.GetFileName(file.FileName);

            string extension = Path.GetExtension(dataFileName);

            string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

            if (!allowedExtsnions.Contains(extension))
                throw new System.Exception(
                "Sorry! This file is not allowed,  make sure that file having extension as either.xls or.xlsx is uploaded.");

            // USe this to handle Encodeing differences in .NET Core
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                if (extension == ".xls")
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                else
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                DataSet ds = new DataSet();
                ds = reader.AsDataSet();
                reader.Close();

                if (ds != null && ds.Tables.Count > 0)
                {
                    // Read the the Table
                    DataTable userDetails = ds.Tables[0];
                    for (int i = 1; i < userDetails.Rows.Count; i++)
                    {
                        User user = new User();
                        user.UserName = userDetails.Rows[i][1].ToString();
                        user.Password = userDetails.Rows[i][2].ToString();
                        user.Email = userDetails.Rows[i][3].ToString();
                        user.FirstName = userDetails.Rows[i][4].ToString();
                        user.LastName = userDetails.Rows[i][5].ToString();
                        user.Gender = int.Parse(userDetails.Rows[i][6].ToString());
                        user.BirthDate = Convert.ToDateTime(userDetails.Rows[i][7].ToString());
                        user.Address = userDetails.Rows[i][8].ToString();
                        user.Phone = userDetails.Rows[i][9].ToString();
                        user.IsTeacher = 0;
                        user.Enable = 0;
                        int roleid = int.Parse(userDetails.Rows[i][10].ToString());
                        string subjectid = userDetails.Rows[i][11] == null ? "" : userDetails.Rows[i][11].ToString();
                        user.CreateBy = userId;
                        user.UpdateBy = userId;
                        user.CreateDate = DateTime.Now;
                        //user.roleId = userDetails.Rows[i][9].ToString();
                        //user.subjectId = Convert.ToDateTime(userDetails.Rows[i][10].ToString());

                        //user.Fees = Convert.ToDecimal(userDetails.Rows[i][11].ToString());
                        ///user.UploadDate = DateTime.Now;

                        var response = await _userService.createUsers(user, roleid, subjectid);
                        if (response.Status == false)
                        {
                            return BadRequest(new ProblemDetails
                            {
                                Status = response.ErrorCode,
                                Title = response.Message
                            });
                        }
                    }
                }
            }

            var responseReturn = new ServiceResponse<User>();

            return Ok(responseReturn);
        }
    }
}
