using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;

namespace VisssStock.Application.Interfaces
{
    public interface IExchangeService
    {
        Task<ServiceResponse<PagedListResponseDTO<ExchangeResponseDTO>>> GetAllExchangeAsync(OwnerParameters ownerParameters);
        Task<ServiceResponse<ExchangeResponseDTO>> CreateExchangeAsync(ExchangeRequestDTO exchangeRequestDTO);
        Task<ServiceResponse<ExchangeResponseDTO>> UpdateExchangeByIdAsync(int exchangeId, ExchangeRequestDTO exchangeRequestDTO);
        Task<ServiceResponse<ExchangeResponseDTO>> DeleteExchangeByIdAsync(int exchangeId);
    }
}
