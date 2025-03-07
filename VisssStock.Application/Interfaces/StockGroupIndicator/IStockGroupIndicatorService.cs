using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Interfaces
{
    public interface IStockGroupIndicatorService
    {
        // get all stock group indicator
        Task<ServiceResponse<PagedListResponseDTO<StockGroupIndicatorResponseDTO>>> getAllStockGroupIndicators(OwnerParameters ownerParameters, string searchByName);

        // get stock group indicator by id
        Task<ServiceResponse<StockGroupIndicatorResponseDTO>> getStockGroupIndicatorById(int id);
        // update stock group indicator
        Task<ServiceResponse<StockGroupIndicatorResponseDTO>> updateStockGroupIndicator(UpdateStockGroupIndicatorDTO updateStockGroupIndicatorDTO, int id);

        // delete stock group indicator
        Task<ServiceResponse<StockGroupIndicatorResponseDTO>> deleteStockGroupIndicator(int id);
        // create stock group indicator
        //Task<ServiceResponse<StockGroupIndicatorResponseDTO>> CreateStockGroupIndicator(List<CreateStockGroupIndicatorDTO> createStockGroupIndicatorDTOs);


        // Thêm dữ liệu vào bảng StockGroupIndicator dựa vào stockGroupId 
        Task<ServiceResponse<List<StockGroupIndicatorResponseDTO>>> CreateUpdateStockGroupIndicatorAsync(int stockGroupId, List<StockGroupIndicatorRequestDTO> stockGroupIndicatorRequestDTOs);

        // Task<ServiceResponse<List<IndicatorResponseDTO>>> GetAllIndicatorByStockGroupId(int stockGroupId);
        // Lấy những Indicator thuộc StockGroup đó, dựa vào bảng StockGroupIndicator
        Task<ServiceResponse<List<StockGroupIndicatorResponseDTO2>>> GetAllStockGroupIndicatorByStockGroupId(int stockGroupId);

        ServiceResponse<bool> CheckFormula(string expression);
    }
}
