using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;

namespace VisssStock.Application.Interfaces
{
    public interface ITypeService
    {
        Task<ServiceResponse<List<TypeResponseDTO>>> GetAllTypeAsync();
        Task<ServiceResponse<TypeResponseDTO>> CreateTypeAsync(TypeRequestDTO typeRequestDTO);
        Task<ServiceResponse<TypeResponseDTO>> UpdateTypeByIdAsync(int typeId, TypeRequestDTO typeRequestDTO);
        Task<ServiceResponse<TypeResponseDTO>> DeleteTypeByIdAsync(int typeId);
    }
}