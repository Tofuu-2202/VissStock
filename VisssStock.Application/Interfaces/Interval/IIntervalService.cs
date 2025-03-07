using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;

namespace VisssStock.Application.Interfaces
{
    public interface IIntervalService
    {
        // get all interval
        Task<ServiceResponse<PagedListResponseDTO<IntervalResponseDTO>>> getAllIntervals(OwnerParameters ownerParameters, string searchByName);

        // get interval by id
        Task<ServiceResponse<IntervalResponseDTO>> getIntervalById(int id);

        // update interval
        Task<ServiceResponse<IntervalResponseDTO>> updateInterval(UpdateIntervalDTO updateIntervalDTO, int id);

        // delete interval
        Task<ServiceResponse<IntervalResponseDTO>> deleteInterval(int id);

        // create interval
        Task<ServiceResponse<IntervalResponseDTO>> createInterval(CreateIntervalDTO createIntervalDTO);

        Task<ServiceResponse<IntervalResponseDTO>> insertIndicatorsTool();
    }
}
