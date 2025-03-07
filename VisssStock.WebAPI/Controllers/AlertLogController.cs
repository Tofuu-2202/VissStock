using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs;
using VisssStock.Application.Services.AlertLogService;
using VisssStock.Application.Services.ConditonGroupService;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AlertLogController : Controller
    {
        private readonly IAlertLogService _alertLogService;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AlertLogController(IAlertLogService alertLogService, DataContext context, IConfiguration configuration)
        {
            _alertLogService = alertLogService;
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("CreateAlertLog")]
        public async Task<ActionResult<AlertLogResponseDto>> CreateAlertLog([FromBody] AlertLogRequestDto alertLogRequestDto)
        {
            var response = await _alertLogService.CreateAlertLog(alertLogRequestDto);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("GetAlertLogByFillter")]
        public async Task<ActionResult<List<AlertLogResponseDto>>> GetAlertLogByFillter([FromQuery] string chatId, [FromBody] AlertLogFillterDto requestDto)
        {
            var response = await _alertLogService.GetAlertLogByFillter(chatId, requestDto);
            return Ok(response);
        }
    }
}
