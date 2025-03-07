using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Interfaces
{
    public interface IIndicatorService
    {
        // get all indicator
        Task<ServiceResponse<PagedListResponseDTO<IndicatorResponseDTO>>> getAllIndicators(OwnerParameters ownerParameters, string searchByName);

        // get indicator by id
        Task<ServiceResponse<IndicatorResponseDTO>> getIndicatorById(int id);

        // update indicator
        Task<ServiceResponse<IndicatorResponseDTO>> updateIndicator(UpdateIndicatorDTO updateIndicatorDTO, int id);

        // delete indicator

        Task<ServiceResponse<IndicatorResponseDTO>> deleteIndicator(int id);

        // create indicator
        Task<ServiceResponse<IndicatorResponseDTO>> createIndicator(CreateIndicatorDTO createIndicatorDTO);
    }
}
