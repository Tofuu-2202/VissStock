using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;

namespace VisssStock.Application.Services
{
    public interface IStockGroupService
    {
        Task<ServiceResponse<PagedListResponseDTO<StockGroupResponseDTO>>> GetAllStockGroupAsync(OwnerParameters ownerParameters, StockGroupFilterDto requestDto);
        Task<ServiceResponse<StockGroupResponseDTO>> GetStockGroupByIdAsync(int stockGroupId);
        Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupAsync(StockGroupRequestDTO stockGroupRequestDTO);
        Task<ServiceResponse<StockGroupResponseDTO>> UpdateStockGroupByIdAsync(int stockGroupId, StockGroupRequestDTO stockGroupRequestDTO);
        Task<ServiceResponse<StockGroupResponseDTO>> UpdateStockGroupStatusByIdAsync(int stockGroupId);
        Task<ServiceResponse<StockGroupResponseDTO>> DeleteStockGroupByIdAsync(int stockGroupId);

        // Many to many với bảng Stock, sử dụng bảng trung gian StockGroupStock
        Task<ServiceResponse<PagedListResponseDTO<StockResponseDTO>>> GetAllStockByStockGroupIdAsync(OwnerParameters ownerParameters, int stockGroupId);
        Task<ServiceResponse<StockGroupResponseDTO>> AddStockToStockGroupByStockGroupId(int stockId, int stockGroupId);
        Task<ServiceResponse<StockGroupResponseDTO>> RemoveStockInStockGroupByStockGroupId(int stockId, int stockGroupId);
        Task<ServiceResponse<List<StockGroupResponseDTO>>> GetAllStockGroupOfUser();
        Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneAsync(int stockGroupId);
        Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneByConditionGroupIdAsync(int conditionGroupId);

        Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneByConditionGroupIdIntoOtherConditionGroupAsync(int conditionGroupIdInput, int conditionGroupIdOutput);
        Task<ServiceResponse<StockGroupResponseDTO>> ReplaceIndicatorOfStockGroupIntoOtherStockGroupAsync(int stockGroupIdInput, int stockGroupIdOutput);
    }
}