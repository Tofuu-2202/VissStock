using System.Security.Claims;
using VisssStock.Application.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VisssStock.Infrastructure.Data;
using VisssStock.Application.Models;
using VisssStock.Application.Services.AuthService;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;


        public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor, DataContext context,
        IConfiguration configuration)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("getMenuItems")]
        public async Task<ActionResult<ServiceResponse<List<MenuDTO>>>> getMenuItems()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return Ok(await _authService.getMenuItems(userId));
        }
              


        [Microsoft.AspNetCore.Mvc.HttpPost("registerSingleUser")]
        public async Task<ActionResult<ServiceResponse<UserDTO>>> registerSingleUser( 
        [Microsoft.AspNetCore.Mvc.FromBody] CreateUserDTO createUserDto)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:AUTH_CREATE_SINGLE_USER").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }


            var response = await _authService.createSingleUser(createUserDto);
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

        [HttpPut("resetPassword")]
        public async Task<ActionResult<ServiceResponse<ResetPasswordForm>>> resetPassword(
        [FromBody] ResetPasswordForm resetPasswordForm)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var response = await _authService.resetPassword(resetPasswordForm, userId);
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

        [HttpPut("resetPasswordAdmin/{id}")]
        public async Task<ActionResult<ServiceResponse<ResetPasswordForm>>> resetPasswordAdmin(
        int id
        )
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

            var response = await _authService.resetPasswordAdmin(id);
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

        [HttpPut("editMyProfile")]
        public async Task<ActionResult<ServiceResponse<UpdateProfileDTO>>> editMyProfile(
        [FromBody] UpdateProfileDTO updateProfileDTO)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var response = await _authService.updateProfile(updateProfileDTO, userId);
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

        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> login([Microsoft.AspNetCore.Mvc.FromBody] LoginFrom loginFrom)
        {
            var response = await _authService.login(loginFrom);
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
    }
}
