using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Services.TelegramBotService;
using VisssStock.Infrastructure.Data;
using VisssStock.Application.DTOs.ProjectDTOs;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TelegramBotController : Controller
    {
        private readonly ITelegramBotService _TelegramBotService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public TelegramBotController(ITelegramBotService TelegramBotService, DataContext context, IConfiguration configuration)
        {
            _TelegramBotService = TelegramBotService;
            _context = context;
            _configuration = configuration;
        }

        // get all TelegramBots
        [HttpGet("getAllTelegramBots")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<TelegramBotResponseDto>>>> getAllTelegramBots([FromQuery] OwnerParameters ownerParameters, string searchByName)
        {
            var response = await _TelegramBotService.getAllTelegramBots(ownerParameters, searchByName);
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

        // create TelegramBot
        [HttpPost("createTelegramBot")]
        public async Task<ActionResult<ServiceResponse<TelegramBotResponseDto>>> createTelegramBot(TelegramBotRequestDto createTelegramBotDTO)
        {
            var response = await _TelegramBotService.createTelegramBot(createTelegramBotDTO);
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

        // delete TelegramBot
        [HttpDelete("deleteTelegramBot/{id}")]
        public async Task<ActionResult<ServiceResponse<TelegramBotResponseDto>>> deleteTelegramBot(int id)
        {
            var response = await _TelegramBotService.deleteTelegramBot(id);
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

        // update TelegramBot
        [HttpPut("updateTelegramBot/{id}")]
        public async Task<ActionResult<ServiceResponse<TelegramBotResponseDto>>> updateTelegramBot(TelegramBotRequestDto updateTelegramBotDTO, int id)
        {
            var response = await _TelegramBotService.updateTelegramBot(updateTelegramBotDTO, id);
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

        // get TelegramBot by id
        [HttpGet("getTelegramBotById/{id}")]
        public async Task<ActionResult<ServiceResponse<TelegramBotResponseDto>>> getTelegramBotById(int id)
        {
            var response = await _TelegramBotService.getTelegramBotById(id);
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

        //Task<ServiceResponse<TelegramBotResponseDto>> Tool()
        [HttpPost("Tool")]
        public async Task<ActionResult<ServiceResponse<TelegramBotResponseDto>>> Tool()
        {
            var response = await _TelegramBotService.Tool();
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
