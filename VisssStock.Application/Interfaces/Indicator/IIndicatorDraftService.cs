using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;

namespace VisssStock.Application.Interfaces
{
    public interface IIndicatorDraftService
    {
        //get all indicator draft
        Task<ServiceResponse<List<IndicatorDraftResponseDto>>> getAllIndicatorDrafts();

        Task<ServiceResponse<List<IndicatorDraftResponseDto>>> getAllIndicatorDrafts(int stockGroupId);

        //create update indicator draft
        Task<ServiceResponse<List<IndicatorDraftResponseDto>>> createUpdateIndicatorDraft(List<IndicatorDraftRequestDto> indicatorDraftRequestDto);

        //delete indicator draft
        Task<ServiceResponse<IndicatorDraftResponseDto>> deleteIndicatorDraft(int id);

        //get all indicator of stock group
        Task<ServiceResponse<List<IndicatorResponseDTO>>> getAllIndicatorByStockGroupId(int stockGroupId);
    }
}
