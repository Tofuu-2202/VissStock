using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Services.ScreenerServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ScreenerController : ControllerBase
    {
        private readonly IScreenerService _screenerService;

        public ScreenerController(IScreenerService screenerService)
        {
            _screenerService = screenerService;
        }

        [HttpGet("GetAllScreenerAsync")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<ScreenerResponseDTO>>>> GetAllScreenerAsync([FromQuery] OwnerParameters ownerParameters)
        {
            var serviceResponse = await _screenerService.GetAllScreenerAsync(ownerParameters);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpGet("GetScreenerByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<ScreenerResponseDTO>>> GetScreenerByIdAsync(int id)
        {
            var serviceResponse = await _screenerService.GetScreenerByIdAsync(id);

            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }

            return Ok(serviceResponse);
        }

        [HttpPost("CreateScreenerAsync")]
        public async Task<ActionResult<ServiceResponse<ScreenerResponseDTO>>> CreateScreenerAsync([FromBody] ScreenerRequestDTO screenerRequestDTO)
        {
            var serviceResponse = await _screenerService.CreateScreenerAsync(screenerRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }


        [HttpPut("UpdateScreenerByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<ScreenerResponseDTO>>> UpdateScreenerByIdAsync(int id, [FromBody] ScreenerRequestDTO screenerRequestDTO)
        {
            var serviceResponse = await _screenerService.UpdateScreenerByIdAsync(id, screenerRequestDTO);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        [HttpDelete("DeleteScreenerByIdAsync/{id}")]
        public async Task<ActionResult<ServiceResponse<ScreenerResponseDTO>>> DeleteScreenerByIdAsync(int id)
        {
            var serviceResponse = await _screenerService.DeleteScreenerByIdAsync(id);
            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }
    }
}