using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Infrastructure.Repository.TransactionRepository;
using VisssStock.Application.Interfaces;
namespace VisssStock.Application.Services.TransactionServices
{
    public class TransactionServices : ITransactionServices
    {
        private readonly TransactionRepository _transactionRepository;

        // Khởi tạo repository qua dependency injection
        public TransactionServices(TransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<ServiceResponse<PagedListResponseDTO<TransactionResponseDto>>> GetAllTransactionAsync(TransactionFillterDto filterDto)
        {
            try
            {
                // Gọi phương thức từ repository
                var transactions = await _transactionRepository.GetAllTransactionAsync(filterDto);

                return ServiceResponse<PagedListResponseDTO<TransactionResponseDto>>.Success(transactions);
            }
            catch (Exception e)
            {
                return ServiceResponse<PagedListResponseDTO<TransactionResponseDto>>.Failure(500, e.Message);
            }
        }

        public async Task<ServiceResponse<TransactionResponseDto>> CreateTransactionAsync(TransactionRequestDto transactionRequestDto)
        {
            try
            {
                var transactions = await _transactionRepository.CreateTransactionAsync(transactionRequestDto);

                return ServiceResponse<TransactionResponseDto>.Success(transactions);
            }
            catch (Exception e)
            {
                return ServiceResponse<TransactionResponseDto>.Failure(500, e.Message);
            }
        }

        public async Task<ServiceResponse<TransactionResponseDto>> UpdateTransactionAsync(int id, TransactionRequestDto transactionRequestDto)
        {
            try
            {
                var transaction = await _transactionRepository.UpdateTransactionAsync(id, transactionRequestDto);

                return ServiceResponse<TransactionResponseDto>.Success(transaction);
            }
            catch (Exception ex) 
            {
                return ServiceResponse<TransactionResponseDto>.Failure(500, ex.Message);
            }
        }

        //delete transaction
        public async Task<ServiceResponse<TransactionResponseDto>> DeleteTransactionAsync(int id)
        {
            try
            {
                await _transactionRepository.DeleteTransactionAsync(id);

                return ServiceResponse<TransactionResponseDto>.Success(null);
            }
            catch (Exception ex)
            {
                return ServiceResponse<TransactionResponseDto>.Failure(500, ex.Message);
            }
        }
    }
}
