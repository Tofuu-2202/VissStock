using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Interfaces
{
    public interface IAlertLogRepository
    {
        Task<AlertLog> CreateAlertLog(AlertLog alertLog);
        Task<List<AlertLog>> GetAlertLogByFilter(string chatId);
    }
}
