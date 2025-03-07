using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.IntervalServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class IntervalController : ControllerBase
    {
        private readonly IIntervalService intervalService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public IntervalController(IIntervalService intervalService, DataContext context, IConfiguration configuration)
        {
            this.intervalService = intervalService;
            _context = context;
            _configuration = configuration;
        }

        // get all intervals
        [HttpGet("getAllIntervals")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<IntervalResponseDTO>>>> getAllIntervals([FromQuery] OwnerParameters ownerParameters, string searchByName)
        {
            var response = await intervalService.getAllIntervals(ownerParameters, searchByName);
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

        // create interval
        [HttpPost("createInterval")]
        public async Task<ActionResult<ServiceResponse<IntervalResponseDTO>>> createInterval(CreateIntervalDTO createIntervalDTO)
        {
            var response = await intervalService.createInterval(createIntervalDTO);
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

        // delete interval
        [HttpDelete("deleteInterval/{id}")]
        public async Task<ActionResult<ServiceResponse<IntervalResponseDTO>>> deleteInterval(int id)
        {
            var response = await intervalService.deleteInterval(id);
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

        // update interval
        [HttpPut("updateInterval/{id}")]
        public async Task<ActionResult<ServiceResponse<IntervalResponseDTO>>> updateInterval(UpdateIntervalDTO updateIntervalDTO, int id)
        {
            var response = await intervalService.updateInterval(updateIntervalDTO, id);
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

        // get interval by id
        [HttpGet("getIntervalById/{id}")]
        public async Task<ActionResult<ServiceResponse<IntervalResponseDTO>>> getIntervalById(int id)
        {
            var response = await intervalService.getIntervalById(id);
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

        //Task<ServiceResponse<IntervalResponseDTO>> insertIndicatorsTool()
        [HttpPost("insertIndicatorsTool")]
        public async Task<ActionResult<ServiceResponse<IntervalResponseDTO>>> insertIndicatorsTool()
        {
            var response = await intervalService.insertIndicatorsTool();
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
