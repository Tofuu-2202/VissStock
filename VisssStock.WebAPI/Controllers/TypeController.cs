using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Services.TypeServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TypeController : ControllerBase
    {
        private readonly ITypeService _typeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public TypeController(ITypeService typeService, IHttpContextAccessor httpContextAccessor, DataContext dataContext, IConfiguration configuration)
        {
            _typeService = typeService;
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
            _configuration = configuration;
        }

        [HttpGet("GetAllTypeAsync")]
        public async Task<ActionResult<ServiceResponse<List<TypeResponseDTO>>>> GetAllTypeAsync()
        {
            var serviceResponse = await _typeService.GetAllTypeAsync();
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpPost("CreateTypeAsync")]
        public async Task<ActionResult<ServiceResponse<TypeResponseDTO>>> CreateTypeAsync([FromBody] TypeRequestDTO typeRequestDTO)
        {
            var serviceResponse = await _typeService.CreateTypeAsync(typeRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpPut("UpdateTypeByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<TypeResponseDTO>>> UpdateTypeByIdAsync(int id, [FromBody] TypeRequestDTO typeRequestDTO)
        {
            var serviceResponse = await _typeService.UpdateTypeByIdAsync(id, typeRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpDelete("DeleteTypeByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<TypeResponseDTO>>> DeleteTypeByIdAsync(int id)
        {
            var serviceResponse = await _typeService.DeleteTypeByIdAsync(id);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }
    }
}