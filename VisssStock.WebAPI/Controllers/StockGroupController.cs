using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Services.StockGroupServices;
using VisssStock.Infrastructure.Data;
using VisssStock.Application.Models.Pagination;

namespace VisssStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockGroupController : ControllerBase
    {
        private readonly IStockGroupService _stockGroupService;

        public StockGroupController(IStockGroupService stockGroupService)
        {
            _stockGroupService = stockGroupService;
        }

        // GET: api/StockGroup/GetAllStockGroupAsync
        [HttpPost("GetAllStockGroupAsync")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<StockGroupResponseDTO>>>> GetAllStockGroupAsync([FromQuery] OwnerParameters ownerParameters, StockGroupFilterDto requestDto)
        {
            var serviceResponse = await _stockGroupService.GetAllStockGroupAsync(ownerParameters, requestDto);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        // GET: api/StockGroup/GetAllStockByStockGroupIdAsync/{stockGroupId}
        [HttpGet("GetAllStockByStockGroupIdAsync/{stockGroupId}")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<StockResponseDTO>>>> GetAllStockByStockGroupIdAsync(int stockGroupId, [FromQuery] OwnerParameters ownerParameters)
        {
            var serviceResponse = await _stockGroupService.GetAllStockByStockGroupIdAsync(ownerParameters, stockGroupId);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        // GET: api/StockGroup/GetStockGroupByIdAsync/{id}
        [HttpGet("GetStockGroupByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> GetStockGroupByIdAsync(int id)
        {
            var serviceResponse = await _stockGroupService.GetStockGroupByIdAsync(id);

            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }

            return Ok(serviceResponse);
        }

        // POST: api/StockGroup/CreateStockGroupAsync
        [HttpPost("CreateStockGroupAsync")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> CreateStockGroupAsync([FromBody] StockGroupRequestDTO stockGroupRequestDTO)
        {
            var serviceResponse = await _stockGroupService.CreateStockGroupAsync(stockGroupRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        // PUT: api/StockGroup/UpdateStockGroupByIdAsync/{id}
        [HttpPut("UpdateStockGroupByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> UpdateStockGroupByIdAsync(int id, [FromBody] StockGroupRequestDTO stockGroupRequestDTO)
        {
            var serviceResponse = await _stockGroupService.UpdateStockGroupByIdAsync(id, stockGroupRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        // PUT: api/StockGroup/UpdateStockGroupByIdAsync/{id}
        [HttpPut("UpdateStockGroupStatusByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> UpdateStockGroupStatusByIdAsync(int id)
        {
            var serviceResponse = await _stockGroupService.UpdateStockGroupStatusByIdAsync(id);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        // DELETE: api/StockGroup/DeleteStockGroupByIdAsync/{id}
        [HttpDelete("DeleteStockGroupByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> DeleteStockGroupByIdAsync(int id)
        {
            var serviceResponse = await _stockGroupService.DeleteStockGroupByIdAsync(id);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        // POST: api/StockGroup/AddStockToStockGroupByStockGroupId/{stockGroupId}/{stockId}
        [HttpPost("AddStockToStockGroupByStockGroupId/{stockGroupId}/{stockId}")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> AddStockToStockGroupByStockGroupId(int stockGroupId, int stockId)
        {
            var serviceResponse = await _stockGroupService.AddStockToStockGroupByStockGroupId(stockId, stockGroupId);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        // DELETE: api/StockGroup/RemoveStockInStockGroupByStockGroupId/{stockGroupId}/{stockId}
        [HttpDelete("RemoveStockInStockGroupByStockGroupId/{stockGroupId}/{stockId}")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> RemoveStockInStockGroupByStockGroupId(int stockGroupId, int stockId)
        {
            var serviceResponse = await _stockGroupService.RemoveStockInStockGroupByStockGroupId(stockId, stockGroupId);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        //Task<ServiceResponse<List<StockGroupResponseDTO>>> GetAllStockGroupOfUser();

        [HttpGet("GetAllStockGroupOfUser")]
        public async Task<ActionResult<ServiceResponse<List<StockGroupResponseDTO>>>> GetAllStockGroupOfUser()
        {
            var serviceResponse = await _stockGroupService.GetAllStockGroupOfUser();
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }


        //Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneAsync(int stockGroupId);

        [HttpPost("CreateStockGroupCloneAsync")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> CreateStockGroupCloneAsync(int stockGroupId)
        {
            var serviceResponse = await _stockGroupService.CreateStockGroupCloneAsync(stockGroupId);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        //Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneByConditionGroupIdAsync(int conditionGroupId)

        [HttpPost("CreateStockGroupCloneByConditionGroupIdAsync")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>> CreateStockGroupCloneByConditionGroupIdAsync(int conditionGroupId)
        {
            var serviceResponse = await _stockGroupService.CreateStockGroupCloneByConditionGroupIdAsync(conditionGroupId);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        //Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneByConditionGroupIdIntoOtherConditionGroupAsync(int conditionGroupIdInput, int conditionGroupIdOutput)

        [HttpPost("CreateStockGroupCloneByConditionGroupIdIntoOtherConditionGroupAsync")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>>
            CreateStockGroupCloneByConditionGroupIdIntoOtherConditionGroupAsync(int conditionGroupIdInput, int conditionGroupIdOutput)
        {
            var serviceResponse = await _stockGroupService.CreateStockGroupCloneByConditionGroupIdIntoOtherConditionGroupAsync(conditionGroupIdInput, conditionGroupIdOutput);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        //        Task<ServiceResponse<StockGroupResponseDTO>> ReplaceIndicatorOfStockGroupIntoOtherStockGroupAsync(int stockGroupIdInput, int stockGroupIdOutput);

        [HttpPost("ReplaceIndicatorOfStockGroupIntoOtherStockGroupAsync")]
        public async Task<ActionResult<ServiceResponse<StockGroupResponseDTO>>>
            ReplaceIndicatorOfStockGroupIntoOtherStockGroupAsync(int stockGroupIdInput, int stockGroupIdOutput)
        {
            var serviceResponse = await _stockGroupService.ReplaceIndicatorOfStockGroupIntoOtherStockGroupAsync(stockGroupIdInput, stockGroupIdOutput);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }
    }
}