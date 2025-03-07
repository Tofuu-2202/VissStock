using System.Security.Claims;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.PermissionServices;
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
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public PermissionController(IPermissionService permissionService, DataContext context, IConfiguration configuration)
        {
            _permissionService = permissionService;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("getAllPermissions")]
        public async Task<ActionResult<ServiceResponse<PagedList<Permission>>>> getAllPermissions(  [FromQuery] OwnerParameters ownerParameters,
        string? searchByName)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:PERMISSION_VIEW_ALL").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }
            var permissions = await _permissionService.getAllPermissions(ownerParameters, searchByName);
            var metadata = new
            {
                permissions.Data.TotalCount,
                permissions.Data.PageSize,
                permissions.Data.CurrentPage,
                permissions.Data.TotalPages,
                permissions.Data.HasNext,
                permissions.Data.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(permissions);
        }
        
        [HttpGet("getOne/{id}")]
        public async Task<ActionResult<ServiceResponse<Permission>>> getOne(int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:PERMISSION_VIEW_DETAIL").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }
            var response = await _permissionService.getPermissionById(id);
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
        public async Task<ActionResult<ServiceResponse<Permission>>> createRole([FromBody] CreatePermissionDTO createPermissionDto)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            
            var response = await _permissionService.createPermission(createPermissionDto);
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
        public async Task<ActionResult<ServiceResponse<Role>>> updatePermission(
        [FromBody] CreatePermissionDTO createPermissionDto, int id)
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var permission = (from u in _context.Users
            join ur in _context.UserRoles on u.Id equals ur.UserId
            join r in _context.Roles on ur.RoleId equals r.Id
            join rp in _context.RolePermissions on r.Id equals rp.RoleId
            join p in _context.Permissions on rp.PermissionId equals p.Id
            where u.Id == userId
            where p.Name == _configuration.GetSection("Permission:PERMISSION_UPDATE").Value
            select p).FirstOrDefault();
            if (permission == null)
            {
                return new StatusCodeResult(403);
            }
            var response = await _permissionService.updatePermission(createPermissionDto, id);
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
