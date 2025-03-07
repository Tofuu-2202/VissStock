using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.IntervalServices;
using VisssStock.Application.Services.TradingViewAPI;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    public class TradingViewController : Controller
    {
        private readonly ITradingViewService _tradingViewService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public TradingViewController(ITradingViewService tradingViewService, IHttpContextAccessor httpContextAccessor, DataContext dataContext, IConfiguration configuration)
        {
            _tradingViewService = tradingViewService;
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
            _configuration = configuration;
        }

        //Task<ServiceResponse<PagedListResponseDTO<IndicatorResponseDTO>>> getAllIndicators(OwnerParameters ownerParameters, string searchByName);

        [HttpPost("GetAllIndicators")]
        public async Task<ActionResult<ServiceResponse<List<IndicatorResponseDTO>>>> getAllIndicators()
        {
            var serviceResponse = await _tradingViewService.getAllIndicators();

            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }
            return Ok(serviceResponse);
        }

        //Task<ServiceResponse<TradingViewApiResponse>> GetTradingViewData(string screener)

        [HttpGet("GetTradingViewData")]
        public async Task<ActionResult<ServiceResponse<TradingViewApiResponse>>> GetTradingViewData(string screener)
        {
            var serviceResponse = await _tradingViewService.GetTradingViewData(screener);

            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }

            return Ok(serviceResponse);
        }

        //Task<ServiceResponse<List<StockDataTradingView>>> GetTradingViewDataStock(string screener, List<string> symbols, List<string> Indicator)

        [HttpPost("GetTradingViewDataStock")]
        public async Task<ActionResult<ServiceResponse<List<StockDataTradingView>>>> GetTradingViewDataStock(TradingViewRequestStock requestStock)
        {
            var serviceResponse = await _tradingViewService.GetTradingViewDataStock(requestStock);

            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }

            return Ok(serviceResponse);
        }

        //Task<ServiceResponse<PagedList<StockGroupResponsexDTO>>> GetAllStockGroupIndicators(OwnerParameters ownerParameters)

        [HttpPost("GetAllStockGroupIndicators")]
        public async Task<ActionResult<ServiceResponse<List<StockGroupResponsexDTO>>>> GetAllStockGroupIndicators()
        {
            var serviceResponse = await _tradingViewService.GetAllStockGroupIndicators();

            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }

            return Ok(serviceResponse);
        }

        //Task<ServiceResponse<List<StockGroupResponsexDTO>>> GetAllStockGroupIndicators(TelegramSaveRequestDTO requestDTO)

        [HttpPost("UpdateUserTelegram")]
        public async Task<ActionResult<ServiceResponse<List<StockGroupResponsexDTO>>>> UpdateUserTelegram(TelegramSaveRequestDTO requestDTO)
        {
            var serviceResponse = await _tradingViewService.UpdateUserTelegram(requestDTO);

            if (!serviceResponse.Status)
            {
                return StatusCode(serviceResponse.ErrorCode, serviceResponse);
            }

            return Ok(serviceResponse);
        }

        [HttpGet("getAllIndicatorDrafts")]
        public async Task<ActionResult<ServiceResponse<List<IndicatorDraftResponseDto>>>> getAllIndicatorDrafts()
        {
            var response = await _tradingViewService.getAllIndicatorDrafts();
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
