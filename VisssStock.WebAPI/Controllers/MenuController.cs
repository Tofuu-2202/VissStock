using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.MenuServices;
using VisssStock.Domain.DataObjects;
using VisssStock.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _iMenuService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;


        public MenuController(IMenuService iMenuService, DataContext context, IConfiguration configuration)
        {
            _iMenuService = iMenuService;
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("getOne/{id}")]
        public async Task<ActionResult<ServiceResponse<Menu>>> getOne(int id)
        {
            var response = await _iMenuService.getMenuById(id);
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

        [HttpGet("getAll")]
        public async Task<ActionResult<ServiceResponse<PagedList<Menu>>>> getAllMenu(
        [FromQuery] OwnerParameters ownerParameters)
        {
            var menus = await _iMenuService.getAllMenus(ownerParameters);
            var metadata = new
            {
                menus.Data.TotalCount,
                menus.Data.PageSize,
                menus.Data.CurrentPage,
                menus.Data.TotalPages,
                menus.Data.HasNext,
                menus.Data.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(menus);
        }
        [HttpGet("getAllMenu")]
        public async Task<ActionResult<ServiceResponse<PagedList<Menu>>>> getAllMenu([FromQuery] OwnerParameters ownerParameters,
        string? searchByName)
        {
            var roles = await _iMenuService.getAllMenus(ownerParameters, searchByName);
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
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<Menu>>> createRole([FromBody] MenuBO createMenuDto)
        {
            var response = await _iMenuService.createMenu(createMenuDto);
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
        public async Task<ActionResult<ServiceResponse<Menu>>> updateMenu(
        [FromBody] MenuBO createMenuDto, int id)
        {
            var response = await _iMenuService.updateMenu(createMenuDto, id);
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
        public async Task<ActionResult<ServiceResponse<Menu>>> deleteMenu(int id)
        {
            var response = await _iMenuService.deleteMenu(id);
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
