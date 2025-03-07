using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Services.ExchangeServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExchangesController : ControllerBase
    {
        private readonly IExchangeService _exchangeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public ExchangesController(IExchangeService exchangeService, IHttpContextAccessor httpContextAccessor, DataContext dataContext, IConfiguration configuration)
        {
            _exchangeService = exchangeService;
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
            _configuration = configuration;
        }

        [HttpGet("GetAllExchangeAsync")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<ExchangeResponseDTO>>>> GetAllExchangeAsync([FromQuery] OwnerParameters ownerParameters)
        {
            var serviceResponse = await _exchangeService.GetAllExchangeAsync(ownerParameters);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpPost("CreateExchangeAsync")]
        public async Task<ActionResult<ServiceResponse<ExchangeResponseDTO>>> CreateExchangeAsync([FromBody] ExchangeRequestDTO exchangeRequestDTO)
        {
            var serviceResponse = await _exchangeService.CreateExchangeAsync(exchangeRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpPut("UpdateExchangeByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<ExchangeResponseDTO>>> UpdateExchangeByIdAsync(int id, [FromBody] ExchangeRequestDTO exchangeRequestDTO)
        {
            var serviceResponse = await _exchangeService.UpdateExchangeByIdAsync(id, exchangeRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpDelete("DeleteExchangeByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<ExchangeResponseDTO>>> DeleteExchangeByIdAsync(int id)
        {
            var serviceResponse = await _exchangeService.DeleteExchangeByIdAsync(id);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }
    }
}
