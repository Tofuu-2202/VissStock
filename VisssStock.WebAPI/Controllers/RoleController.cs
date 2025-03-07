using System.Security.Claims;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.RoleServices;
using VisssStock.Domain.DataObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]

    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {

        private readonly IRoleService _roleServices;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public RoleController(IRoleService roleServices, DataContext context, IConfiguration configuration)
        {
            _roleServices = roleServices;
            _context = context;
            _configuration = configuration;
        }

        
        [HttpGet("getAll")]
        public async Task<ActionResult<ServiceResponse<PagedList<Role>>>> getAllRoles([FromQuery] OwnerParameters ownerParameters,
        string? searchByName)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:ROLE_VIEW_ALL").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }
            var roles = await _roleServices.getAllRoles(ownerParameters, searchByName);
            var metadata = new
            {
                roles.Data.TotalCount,
                roles.Data.PageSize,
                roles.Data.CurrentPage,
                roles.Data.TotalPages,
                roles.Data.HasNext,
                roles.Data.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(roles);
        }

        [HttpGet("getOne/{id}")]
        public async Task<ActionResult<ServiceResponse<RoleDTOResponse>>> getOne(int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:ROLE_VIEW_DETAIL").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }
            var response = await _roleServices.getRoleById(id);
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
        
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<Role>>> createRole([FromBody] RoleDTO createRoleDto)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:ROLE_UPDATE").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }
            var response = await _roleServices.createRole(createRoleDto);
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
        public async Task<ActionResult<ServiceResponse<RoleDTO>>> updateRole(
        [FromBody] RoleDTO createRoleDto, int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:ROLE_UPDATE").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }
            var response = await _roleServices.updateRole(createRoleDto, id);
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
        public async Task<ActionResult<ServiceResponse<Role>>> deleteRole(int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:ROLE_UPDATE").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }
            var response = await _roleServices.deleteRole(id);
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
