using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;

namespace VisssStock.Application.Interfaces
{
    public interface IStockService
    {
        Task<ServiceResponse<PagedList<StockResponseDTO>>> GetAllStockAsync(OwnerParameters ownerParameters);
        Task<ServiceResponse<PagedListResponseDTO<StockResponseDTO>>> GetAllStockAsync(OwnerParameters ownerParameters, string name);
        Task<ServiceResponse<StockResponseDTO>> GetOneStockByIdAsync(int stockId);
        Task<ServiceResponse<StockResponseDTO>> CreateStockAsync(StockRequestDTO stockRequestDTO);
        Task<ServiceResponse<StockResponseDTO>> UpdateStockByIdAsync(int stockId, StockRequestDTO stockRequestDTO);
        Task<ServiceResponse<StockResponseDTO>> DeleteStockByIdAsync(int stockId);
        Task<ServiceResponse<List<StockResponseDTO>>> GetAllStockAsyncForStockGroup(string name);

    }
}