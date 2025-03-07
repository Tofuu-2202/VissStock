using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.StockGroupStockServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockGroupStockController : ControllerBase
    {
        private readonly IStockGroupStockService _stockGroupStockService;

        public StockGroupStockController(IStockGroupStockService stockGroupStockService)
        {
            _stockGroupStockService = stockGroupStockService;
        }

        [HttpGet("GetAllStockGroupStockAsync")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<StockGroupStockResponseDTO>>>> GetAllStockGroupStockAsync([FromQuery] OwnerParameters ownerParameters)
        {
            var serviceResponse = await _stockGroupStockService.GetAllStockGroupStockAsync(ownerParameters);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpGet("GetStockGroupStockByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupStockResponseDTO>>> GetStockGroupStockByIdAsync(int id)
        {
            var serviceResponse = await _stockGroupStockService.GetStockGroupStockByIdAsync(id);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpPost("CreateStockGroupStockAsync")]
        public async Task<ActionResult<ServiceResponse<List<StockGroupStockResponseDTO>>>> CreateStockGroupStockAsync([FromBody] List<StockGroupStockRequestDTO> stockGroupStockRequestDTOs)
        {
            var serviceResponse = await _stockGroupStockService.CreateStockGroupStockAsync(stockGroupStockRequestDTOs);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }


        [HttpPut("UpdateStockGroupStockByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupStockResponseDTO>>> UpdateStockGroupStockByIdAsync(int id, [FromBody] StockGroupStockRequestDTO stockGroupStockRequestDTO)
        {
            var serviceResponse = await _stockGroupStockService.UpdateStockGroupStockByIdAsync(id, stockGroupStockRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpDelete("DeleteStockGroupStockByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupStockResponseDTO>>> DeleteStockGroupStockByIdAsync(int id)
        {
            var serviceResponse = await _stockGroupStockService.DeleteStockGroupStockByIdAsync(id);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }


        [HttpPost("CreateUpdateStockInStockGroupAsync/{stockGroupId}")]
        public async Task<ActionResult<ServiceResponse<List<StockGroupStockResponseDTO>>>> CreateStockGroupStockAsync(int stockGroupId,[FromBody] List<int> stockIds)
        {
            var serviceResponse = await _stockGroupStockService.CreateUpdateStockInStockGroupAsync(stockGroupId,stockIds);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        //Task<ServiceResponse<List<StockGroupStockResponseDTO>>> AddToFavoriteStock(int stockGroupStockId, int isFavorite)
        [HttpPost("AddToFavoriteStock/{stockGroupStockId}")]
        public async Task<ActionResult<ServiceResponse<List<StockGroupStockResponseDTO>>>> AddToFavoriteStock(int stockGroupStockId, int isFavorite)
        {
            var serviceResponse = await _stockGroupStockService.AddToFavoriteStock(stockGroupStockId, isFavorite);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }
    }
}