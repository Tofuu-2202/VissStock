using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;

namespace VisssStock.Application.Interfaces
{
    public interface IStockGroupStockService
    {
        Task<ServiceResponse<PagedListResponseDTO<StockGroupStockResponseDTO>>> GetAllStockGroupStockAsync(OwnerParameters ownerParameters);
        Task<ServiceResponse<StockGroupStockResponseDTO>> GetStockGroupStockByIdAsync(int stockGroupStockId);
        Task<ServiceResponse<List<StockGroupStockResponseDTO>>> CreateStockGroupStockAsync(List<StockGroupStockRequestDTO> stockGroupStockRequestDTOs);
        Task<ServiceResponse<StockGroupStockResponseDTO>> UpdateStockGroupStockByIdAsync(int stockGroupStockId, StockGroupStockRequestDTO stockGroupStockRequestDTO);
        Task<ServiceResponse<StockGroupStockResponseDTO>> DeleteStockGroupStockByIdAsync(int stockGroupStockId);
        Task<ServiceResponse<List<StockGroupStockResponseDTO>>> CreateUpdateStockInStockGroupAsync(int stockGroupId, List<int> stockIds);
        Task<ServiceResponse<List<StockGroupStockResponseDTO>>> AddToFavoriteStock(int stockGroupStockId, int isFavorite);
    }
}
