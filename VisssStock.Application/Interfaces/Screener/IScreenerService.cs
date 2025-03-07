using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;

namespace VisssStock.Application.Interfaces
{
    public interface IScreenerService
    {
        Task<ServiceResponse<PagedListResponseDTO<ScreenerResponseDTO>>> GetAllScreenerAsync(OwnerParameters ownerParameters);
        Task<ServiceResponse<ScreenerResponseDTO>> GetScreenerByIdAsync(int screenerId);
        Task<ServiceResponse<ScreenerResponseDTO>> CreateScreenerAsync(ScreenerRequestDTO screenerRequestDTO);
        Task<ServiceResponse<ScreenerResponseDTO>> UpdateScreenerByIdAsync(int screenerId, ScreenerRequestDTO screenerRequestDTO);
        Task<ServiceResponse<ScreenerResponseDTO>> DeleteScreenerByIdAsync(int screenerId);
    }
}