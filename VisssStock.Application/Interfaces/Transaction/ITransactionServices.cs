using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;

namespace VisssStock.Application.Interfaces
{
    public interface ITransactionServices
    {
        Task<ServiceResponse<PagedListResponseDTO<TransactionResponseDto>>> GetAllTransactionAsync(TransactionFillterDto filterDto);

        Task<ServiceResponse<TransactionResponseDto>> CreateTransactionAsync(TransactionRequestDto transactionRequestDto);

        Task<ServiceResponse<TransactionResponseDto>> UpdateTransactionAsync(int id, TransactionRequestDto transactionRequestDto);

        Task<ServiceResponse<TransactionResponseDto>> DeleteTransactionAsync(int id);
    }
}
