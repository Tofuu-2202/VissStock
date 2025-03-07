using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Services.StockServices;
using VisssStock.Application.Services.TransactionServices;

namespace VisssStock.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionServices _transactionServices;

        public TransactionController(ITransactionServices transactionServices)
        {
            _transactionServices = transactionServices;
        }

        //Task<ServiceResponse<PagedListResponseDTO<TransactionResponseDto>>> GetAllTransactionAsync(TransactionFillterDto filterDto)
        [HttpGet("GetAllTransactionAsync")]
        public async Task<IActionResult> GetAllTransactionAsync([FromQuery] TransactionFillterDto filterDto)
        {
            var response = await _transactionServices.GetAllTransactionAsync(filterDto);
            return Ok(response);
        }

        //Task<ServiceResponse<TransactionResponseDto>> CreateTransactionAsync(TransactionRequestDto transactionRequestDto);
        [HttpPost("CreateTransactionAsync")]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] TransactionRequestDto transactionRequestDto)
        {
            var response = await _transactionServices.CreateTransactionAsync(transactionRequestDto);
            return Ok(response);
        }

        //Task<ServiceResponse<TransactionResponseDto>> UpdateTransactionAsync(int id, TransactionRequestDto transactionRequestDto);
        [HttpPut("UpdateTransactionAsync/{id}")]
        public async Task<IActionResult> UpdateTransactionAsync(int id, [FromBody] TransactionRequestDto transactionRequestDto)
        {
            var response = await _transactionServices.UpdateTransactionAsync(id, transactionRequestDto);
            return Ok(response);
        }

        //Task<ServiceResponse<TransactionResponseDto>> DeleteTransactionAsync(int id
        [HttpDelete("DeleteTransactionAsync/{id}")]
        public async Task<IActionResult> DeleteTransactionAsync(int id)
        {
            var response = await _transactionServices.DeleteTransactionAsync(id);
            return Ok(response);
        }
    }
}
