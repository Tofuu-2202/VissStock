using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Services.IndicatorDraft;
using VisssStock.Application.Services.IntervalServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class IndicatorDraftController : Controller
    {
        private readonly IIndicatorDraftService intervalService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public IndicatorDraftController(IIndicatorDraftService intervalService, DataContext context, IConfiguration configuration)
        {
            this.intervalService = intervalService;
            _context = context;
            _configuration = configuration;
        }

        //Task<ServiceResponse<List<IndicatorDraftResponseDto>>> getAllIndicatorDrafts();
        [AllowAnonymous]
        [HttpGet("getAllIndicatorDrafts")]
        public async Task<ActionResult<ServiceResponse<List<IndicatorDraftResponseDto>>>> getAllIndicatorDrafts()
        {
            var response = await intervalService.getAllIndicatorDrafts();
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

        //Task<ServiceResponse<List<IndicatorDraftResponseDto>>> createUpdateIndicatorDraft(List<IndicatorDraftRequestDto> indicatorDraftRequestDto);
        [HttpPost("createUpdateIndicatorDraft")]
        public async Task<ActionResult<ServiceResponse<List<IndicatorDraftResponseDto>>>> createUpdateIndicatorDraft([FromBody] List<IndicatorDraftRequestDto> indicatorDraftRequestDto)
        {
            var response = await intervalService.createUpdateIndicatorDraft(indicatorDraftRequestDto);
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

        //delete indicator draft
        //Task<ServiceResponse<IndicatorDraftResponseDto>> deleteIndicatorDraft(int id);
        [HttpDelete("deleteIndicatorDraft/{id}")]
        public async Task<ActionResult<ServiceResponse<IndicatorDraftResponseDto>>> deleteIndicatorDraft(int id)
        {
            var response = await intervalService.deleteIndicatorDraft(id);
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

        //Task<ServiceResponse<List<IndicatorResponseDTO>>> getAllIndicatorByStockGroupId(int stockGroupId);
        [HttpGet("getAllIndicatorByStockGroupId/{stockGroupId}")]
        public async Task<ActionResult<ServiceResponse<List<IndicatorResponseDTO>>>> getAllIndicatorByStockGroupId(int stockGroupId)
        {
            var response = await intervalService.getAllIndicatorByStockGroupId(stockGroupId);
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


        //public async Task<ServiceResponse<List<IndicatorDraftResponseDto>>> getAllIndicatorDrafts(int stockGroupId)
        [HttpGet("getAllIndicatorDrafts/{stockGroupId}")]
        public async Task<ActionResult<ServiceResponse<List<IndicatorDraftResponseDto>>>> getAllIndicatorDrafts(int stockGroupId)
        {
            var response = await intervalService.getAllIndicatorDrafts(stockGroupId);
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
