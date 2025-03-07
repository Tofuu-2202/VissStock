using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.IndicatorServices;
using VisssStock.Application.Services.UserServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class IndicatorController : ControllerBase
    {
        private readonly IIndicatorService _indicatorService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public IndicatorController(IIndicatorService indicatorService, DataContext context, IConfiguration configuration)
        {
            _indicatorService = indicatorService;
            _context = context;
            _configuration = configuration;
        }

        // get all indicators
        [HttpGet("getAllIndicators")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<IndicatorResponseDTO>>>> getAllIndicators([FromQuery] OwnerParameters ownerParameters, string searchByName)
        {
            var response = await _indicatorService.getAllIndicators(ownerParameters, searchByName);
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

        // create indicator
        [HttpPost("createIndicator")]
        public async Task<ActionResult<ServiceResponse<IndicatorResponseDTO>>> createIndicator(CreateIndicatorDTO createIndicatorDTO)
        {
            var response = await _indicatorService.createIndicator(createIndicatorDTO);
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
        // delete indicator
        [HttpDelete("deleteIndicator/{id}")]
        public async Task<ActionResult<ServiceResponse<IndicatorResponseDTO>>> deleteIndicator(int id)
        {
            var response = await _indicatorService.deleteIndicator(id);
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

        // update indicator
        [HttpPut("updateIndicator/{id}")]
        public async Task<ActionResult<ServiceResponse<IndicatorResponseDTO>>> updateIndicator(UpdateIndicatorDTO updateIndicatorDTO, int id)
        {
            var response = await _indicatorService.updateIndicator(updateIndicatorDTO, id);
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

        // get indicator by id
        [HttpGet("getIndicatorById/{id}")]
        public async Task<ActionResult<ServiceResponse<IndicatorResponseDTO>>> getIndicatorById(int id)
        {
            var response = await _indicatorService.getIndicatorById(id);
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
