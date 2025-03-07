using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.StockServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet("GetAllStockAsync")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<StockResponseDTO>>>> GetAllStockAsync([FromQuery] OwnerParameters ownerParameters, string name)
        {
            var serviceResponse = await _stockService.GetAllStockAsync(ownerParameters, name);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }



        [HttpGet("GetOneStockByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockResponseDTO>>> GetOneStockByIdAsync(int id)
        {
            var serviceResponse = await _stockService.GetOneStockByIdAsync(id);

            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }

            return Ok(serviceResponse);
        }

        [HttpPost("CreateStockAsync")]
        public async Task<ActionResult<ServiceResponse<StockResponseDTO>>> CreateStockAsync([FromBody] StockRequestDTO stockRequestDTO)
        {
            var serviceResponse = await _stockService.CreateStockAsync(stockRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpPut("UpdateStockByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockResponseDTO>>> UpdateStockByIdAsync(int id, [FromBody] StockRequestDTO stockRequestDTO)
        {
            var serviceResponse = await _stockService.UpdateStockByIdAsync(id, stockRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpDelete("DeleteStockByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<StockResponseDTO>>> DeleteStockByIdAsync(int id)
        {
            var serviceResponse = await _stockService.DeleteStockByIdAsync(id);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        //        Task<ServiceResponse<List<StockResponseDTO>>> GetAllStockAsyncForStockGroup(string name);

        [HttpGet("GetAllStockAsyncForStockGroup")]
        public async Task<ActionResult<ServiceResponse<List<StockResponseDTO>>>> GetAllStockAsyncForStockGroup(string name)
        {
            var serviceResponse = await _stockService.GetAllStockAsyncForStockGroup(name);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }
    }
}