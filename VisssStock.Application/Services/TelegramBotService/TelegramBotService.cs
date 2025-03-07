using AutoMapper;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Models;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using VisssStock.Application.DTOs.ProjectDTOs;
using Microsoft.EntityFrameworkCore;
using VisssStock.Application.Interfaces;
// using ZstdSharp.Unsafe;
using ZstdSharp;

namespace VisssStock.Application.Services.TelegramBotService
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TelegramBotService(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<TelegramBotResponseDto>> createTelegramBot(TelegramBotRequestDto requestDto)
        {
            var telegram = _mapper.Map<TelegramBot>(requestDto);
            await _context.TelegramBots.AddAsync(telegram);
            await _context.SaveChangesAsync();
            return ServiceResponse<TelegramBotResponseDto>.Success(_mapper.Map<TelegramBotResponseDto>(telegram));
        }

        public async Task<ServiceResponse<TelegramBotResponseDto>> deleteTelegramBot(int id)
        {
            try
            {
                var TelegramBot = await _context.TelegramBots.FirstOrDefaultAsync(x => x.Id == id);
                if (TelegramBot == null)
                {
                    return ServiceResponse<TelegramBotResponseDto>.Failure(404, "TelegramBot not found.");
                }
                _context.TelegramBots.Remove(TelegramBot);

                await _context.SaveChangesAsync();
                return ServiceResponse<TelegramBotResponseDto>.Success(_mapper.Map<TelegramBotResponseDto>(TelegramBot));

            }
            catch (Exception ex)
            {
                return ServiceResponse<TelegramBotResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<PagedListResponseDTO<TelegramBotResponseDto>>> getAllTelegramBots(OwnerParameters ownerParameters, string searchByName)
        {
            try
            {
                // Truy vấn dữ liệu từ bảng TelegramBots
                IQueryable<TelegramBot> dbTelegramBot = _context.TelegramBots;

                // Lọc dữ liệu theo tên nếu có
                if (!string.IsNullOrEmpty(searchByName))
                {
                    dbTelegramBot = dbTelegramBot
                        .Where(x => x.TelegramBotName.ToLower().Contains(searchByName.ToLower()));
                }

                // Áp dụng phân trang với PagedList
                var TelegramBots = await dbTelegramBot.ToListAsync();
                var TelegramBotResponseDTOs = _mapper.Map<List<TelegramBotResponseDto>>(TelegramBots);
                var pagedTelegramBots = PagedList<TelegramBotResponseDto>.ToPagedList(TelegramBotResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);

                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<TelegramBotResponseDto>(pagedTelegramBots);
                return ServiceResponse<PagedListResponseDTO<TelegramBotResponseDto>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<TelegramBotResponseDto>>.Failure(500, ex.Message);
            }
        }


        public async Task<ServiceResponse<TelegramBotResponseDto>> getTelegramBotById(int id)
        {

            var TelegramBot = await _context.TelegramBots.FirstOrDefaultAsync(x => x.Id == id);
            if (TelegramBot == null)
            {
                return ServiceResponse<TelegramBotResponseDto>.Failure(404, "TelegramBot not found.");
            }
            return ServiceResponse<TelegramBotResponseDto>.Success(_mapper.Map<TelegramBotResponseDto>(TelegramBot));
        }

        public async Task<ServiceResponse<TelegramBotResponseDto>> updateTelegramBot(TelegramBotRequestDto updateTelegramBotDTO, int id)
        {
            {
                var TelegramBot = await _context.TelegramBots.FirstOrDefaultAsync(x => x.Id == id);
                if (TelegramBot == null)
                {
                    return ServiceResponse<TelegramBotResponseDto>.Failure(404, "TelegramBot not found.");
                }

                // Sử dụng AutoMapper để cập nhật các thuộc tính của đối tượng hiện có
                _mapper.Map(updateTelegramBotDTO, TelegramBot);

                // Đánh dấu đối tượng là đã bị thay đổi
                _context.TelegramBots.Update(TelegramBot);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return ServiceResponse<TelegramBotResponseDto>.Success(_mapper.Map<TelegramBotResponseDto>(TelegramBot));
            }
        }

        public async Task<ServiceResponse<TelegramBotResponseDto>> Tool()
        {

            var stockgr = _context.StockGroups.ToList();
            foreach (var stockgroup in stockgr)
            {
                stockgroup.TelegramBotId = 1;
                _context.StockGroups.Update(stockgroup);
            }
            await _context.SaveChangesAsync();
            return ServiceResponse<TelegramBotResponseDto>.Success(null);

        }
    }
}
