using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Services.ConditonGroupService;
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
    public class ConditionGroupController : Controller
    {
        private readonly IConditionGroupService _conditionGroupService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public ConditionGroupController(IConditionGroupService conditionGroupService, DataContext context, IConfiguration configuration)
        {
            _conditionGroupService = conditionGroupService;
            _context = context;
            _configuration = configuration;
        }

        //Task<ServiceResponse<ConditionGroupPagedListResponseDto>> getAllConditionGroups(OwnerParameters ownerParameters);

        [HttpPost("getAllConditionGroups")]

        public async Task<ActionResult<ServiceResponse<ConditionGroupPagedListResponseDto>>> getAllConditionGroups([FromQuery] OwnerParameters ownerParameters,[FromBody] ConditionGroupFillterDto requestDto)
        {
            var response = await _conditionGroupService.getAllConditionGroups(ownerParameters, requestDto);
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

        //Task<ServiceResponse<ConditionGroupResponseDto>> createConditionGroup(ConditionGroupRequestDto conditionGroupRequestDto);

        [HttpPost("createConditionGroup")]
        public async Task<ActionResult<ServiceResponse<ConditionGroupResponseDto>>> createConditionGroup([FromBody] ConditionGroupRequestDto conditionGroupRequestDto)
        {
            var response = await _conditionGroupService.createConditionGroup(conditionGroupRequestDto);
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


        //Task<ServiceResponse<ConditionGroupResponseDto>> updateConditionGroup(int id, ConditionGroupRequestDto conditionGroupRequestDto);

        [HttpPut("updateConditionGroup/{id}")]
        public async Task<ActionResult<ServiceResponse<ConditionGroupResponseDto>>> updateConditionGroup(int id, [FromBody] ConditionGroupRequestDto conditionGroupRequestDto)
        {
            var response = await _conditionGroupService.updateConditionGroup(id, conditionGroupRequestDto);
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


        //Task<ServiceResponse<ConditionGroupResponseDto>> deleteConditionGroup(int id);

        [HttpDelete("deleteConditionGroup/{id}")]
        public async Task<ActionResult<ServiceResponse<ConditionGroupResponseDto>>> deleteConditionGroup(int id)
        {
            var response = await _conditionGroupService.deleteConditionGroup(id);
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
