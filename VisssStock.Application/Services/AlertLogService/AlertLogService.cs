using AutoMapper;
using VisssStock.Application.DTOs;
using VisssStock.Application.Interfaces;
using VisssStock.Application.Models;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Services.AlertLogService
{
    public class AlertLogService : IAlertLogService
    {
        private readonly IAlertLogRepository _alertLogRepository;
        private readonly IMapper _mapper;

        public AlertLogService(IAlertLogRepository alertLogRepository, IMapper mapper)
        {
            _alertLogRepository = alertLogRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AlertLogResponseDto>> CreateAlertLog(AlertLogRequestDto alertLogRequestDto)
        {
            try
            {
                var alertLog = _mapper.Map<AlertLog>(alertLogRequestDto);
                alertLog.CreateAt = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var savedAlertLog = await _alertLogRepository.CreateAlertLog(alertLog);

                return ServiceResponse<AlertLogResponseDto>.Success(_mapper.Map<AlertLogResponseDto>(savedAlertLog));
            }
            catch (Exception ex)
            {
                return ServiceResponse<AlertLogResponseDto>.Failure(500, ex.Message);
            }
        }
        
        public async Task<ServiceResponse<List<AlertLogResponseDto>>> GetAlertLogByFilter(string chatId, AlertLogFillterDto requestDto)
        {
            try
            {
                var alertLogs = await _alertLogRepository.GetAlertLogByFilter(chatId);
                var alertLogResponse = _mapper.Map<List<AlertLogResponseDto>>(alertLogs);

                // Áp dụng bộ lọc
                if (requestDto.FromTime > 0)
                    alertLogResponse = alertLogResponse.Where(x => x.CreateAt >= requestDto.FromTime).ToList();

                if (!string.IsNullOrEmpty(requestDto.Guid))
                    alertLogResponse = alertLogResponse.Where(x => x.Guid.ToLower().Trim() == requestDto.Guid.ToLower().Trim()).ToList();

                alertLogResponse = alertLogResponse
                    .Select(log =>
                    {
                        log.Data.DataJson = log.Data.DataJson
                            .Where(x =>
                                (string.IsNullOrEmpty(requestDto.Indicator) || x.IndicatorDataJson.Any(c => c.Indicator.ToLower().Trim() == requestDto.Indicator.ToLower().Trim())) &&
                                (string.IsNullOrEmpty(requestDto.ConditionGroup) || (!string.IsNullOrEmpty(x.ConditionGroup) && x.ConditionGroup.ToLower().Trim() == requestDto.ConditionGroup.ToLower().Trim())) &&
                                (string.IsNullOrEmpty(requestDto.Interval) || x.Interval.ToLower().Trim() == requestDto.Interval.ToLower().Trim()) &&
                                (string.IsNullOrEmpty(requestDto.Symbol) || x.Symbol.ToLower().Trim() == requestDto.Symbol.ToLower().Trim()) &&
                                (string.IsNullOrEmpty(requestDto.StockGroup) || x.StockGroup.ToLower().Trim() == requestDto.StockGroup.ToLower().Trim())
                            )
                            .ToList();
                        return log;
                    })
                    .Where(log => log.Data.DataJson.Any()) // Lọc ra các log không có DataJson phù hợp
                    .ToList();

                return ServiceResponse<List<AlertLogResponseDto>>.Success(alertLogResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<AlertLogResponseDto>>.Failure(500, ex.Message);
            }
        }


    }
}