using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;

namespace VisssStock.Application.Interfaces
{
    public interface ITradingViewService
    {
        Task<ServiceResponse<TradingViewApiResponse>> GetTradingViewData(string screener);

        Task<ServiceResponse<List<StockDataTradingView>>> GetTradingViewDataStock(TradingViewRequestStock request);

        Task<ServiceResponse<List<StockGroupResponsexDTO>>> GetAllStockGroupIndicators();

        Task<ServiceResponse<List<StockGroupResponsexDTO>>> UpdateUserTelegram(TelegramSaveRequestDTO requestDTO);

        Task<ServiceResponse<List<IndicatorResponseDTO>>> getAllIndicators();

        Task<ServiceResponse<List<IndicatorDraftResponseDto>>> getAllIndicatorDrafts();
    }
}
