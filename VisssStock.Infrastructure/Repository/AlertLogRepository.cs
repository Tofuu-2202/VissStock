using AutoMapper;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VisssStock.Application.DTOs;
using VisssStock.Application.Utility;
using VisssStock.Infrastructure.Data;
using VisssStock.Application.Interfaces;
using VisssStock.Domain.DataObjects;
using VisssStock.Application.Models;

namespace VisssStock.Infrastructure.Repository
{
    public class AlertLogRepository : IAlertLogRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AlertLogRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AlertLog> CreateAlertLog(AlertLog alertLog)
        {
            await _context.AlertLogs.AddAsync(alertLog);
            await _context.SaveChangesAsync();
            return alertLog;
        }

        public async Task<List<AlertLog>> GetAlertLogByFilter(string chatId)
        {
            return await _context.AlertLogs
                .Where(x => x.ChatId.ToLower().Trim() == chatId.ToLower().Trim())
                .ToListAsync();
        }
    }
}
