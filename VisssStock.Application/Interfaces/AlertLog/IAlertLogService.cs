using VisssStock.Application.DTOs;
using VisssStock.Application.Models;

namespace VisssStock.Application.Interfaces
{
    public interface IAlertLogService
    {
        Task<ServiceResponse<List<AlertLogResponseDto>>> GetAlertLogByFilter(string chatId, AlertLogFillterDto requestDto);

        Task<ServiceResponse<AlertLogResponseDto>> CreateAlertLog(AlertLogRequestDto alertLogRequestDto);
    }
}
