using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using VisssStock.Application.Interfaces;


namespace VisssStock.Application.Services.IndicatorDraft
{
    public class IndicatorDraftService : IIndicatorDraftService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndicatorDraftService(DataContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<List<IndicatorDraftResponseDto>>> createUpdateIndicatorDraft(List<IndicatorDraftRequestDto> indicatorDraftRequestDtos)
        {
            try
            {
                //validate
                var stockGroup = await _context.StockGroups.Where(sg => indicatorDraftRequestDtos.Select(c => c.StockGroupId).ToList().Contains(sg.Id)).ToListAsync();

                if (stockGroup.Count != indicatorDraftRequestDtos.Select(c => c.StockGroupId).Distinct().Count())
                {
                    return ServiceResponse<List<IndicatorDraftResponseDto>>.Failure(400, "StockGroupId không tồn tại");
                }

                var listIndicatorId = indicatorDraftRequestDtos.Select(c => c.IndicatorId1).ToList();
                listIndicatorId.AddRange(indicatorDraftRequestDtos.Select(c => c.IndicatorId2).ToList());
                listIndicatorId.Distinct().ToList();

                var indicators = await _context.Indicators.Where(i => listIndicatorId.Contains(i.Id)).ToListAsync();

                if (indicators.Count != listIndicatorId.Distinct().Count())
                {
                    return ServiceResponse<List<IndicatorDraftResponseDto>>.Failure(400, "IndicatorId không tồn tại");
                }

                var indicatorDraftDtos = _mapper.Map<List<Domain.DataObjects.IndicatorDraft>>(indicatorDraftRequestDtos);
                await _context.IndicatorDrafts.AddRangeAsync(indicatorDraftDtos);

                // B2: Xóa cứng tất cả các bản ghi của StockGroupIndicator có stockGroupId như stockGroupId truyền vào
                var indicatorDrafts = await _context.IndicatorDrafts
                    .Where(sgi => sgi.StockGroupId == indicatorDraftRequestDtos.FirstOrDefault().StockGroupId)
                    .ToListAsync();
                _context.IndicatorDrafts.RemoveRange(indicatorDrafts);
                await _context.SaveChangesAsync();

                return ServiceResponse<List<IndicatorDraftResponseDto>>.Success(_mapper.Map<List<IndicatorDraftResponseDto  >>(indicatorDraftDtos));
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<IndicatorDraftResponseDto>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<IndicatorDraftResponseDto>> deleteIndicatorDraft(int id)
        {
            var indicatorDraft = await _context.IndicatorDrafts.FirstOrDefaultAsync(sgi => sgi.Id == id);

            if (indicatorDraft == null)
            {
                return ServiceResponse<IndicatorDraftResponseDto>.Failure(400, "IndicatorDraft không tồn tại");
            }

            _context.IndicatorDrafts.Remove(indicatorDraft);
            await _context.SaveChangesAsync();

            return ServiceResponse<IndicatorDraftResponseDto>.Success(_mapper.Map<IndicatorDraftResponseDto>(indicatorDraft));
        }

        public async Task<ServiceResponse<List<IndicatorResponseDTO>>> getAllIndicatorByStockGroupId(int stockGroupId)
        {
            try
            {
                var userId = GetCurrentUserId();

                // Lấy danh sách StockGroupIndicator dựa vào StockGroupId từ bảng StockGroupIndicator
                var stockGroupIndicators = await _context.StockGroupIndicators
                    .Include(sgi => sgi.Indicator)
                    .Where(sgi => sgi.StockGroupId == stockGroupId && sgi.IsDeleted == 0 && sgi.CreateBy == userId)
                    .ToListAsync();

                var indicators = stockGroupIndicators.Select(sgi => sgi.Indicator).ToList();

                // Map danh sách StockGroupIndicator sang StockGroupIndicatorResponseDTO2
                var stockGroupIndicatorResponseDTOs = _mapper.Map<List<IndicatorResponseDTO>>(indicators);

                // Trả về kết quả
                return ServiceResponse<List<IndicatorResponseDTO>>.Success(stockGroupIndicatorResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<IndicatorResponseDTO>>.Failure(500, ex.Message);
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<ServiceResponse<List<IndicatorDraftResponseDto>>> getAllIndicatorDrafts()
        {
            var indicatorDraftsQuery = from indicatorDraft in _context.IndicatorDrafts
                                  join stockGroup in _context.StockGroups on indicatorDraft.StockGroupId equals stockGroup.Id
                                  join indicator1 in _context.Indicators on indicatorDraft.IndicatorId1 equals indicator1.Id
                                  join indicator2 in _context.Indicators on indicatorDraft.IndicatorId2 equals indicator2.Id
                                  select new IndicatorDraftResponseDto
                                  {
                                      Id = indicatorDraft.Id,
                                      StockGroupId = indicatorDraft.StockGroupId,
                                      IndicatorId1 = indicatorDraft.IndicatorId1,
                                      IndicatorId2 = indicatorDraft.IndicatorId2,
                                      Type = indicatorDraft.Type,
                                      IndicatorId1Navigation = _mapper.Map<IndicatorResponseDTO>(indicator1),
                                      IndicatorId2Navigation = _mapper.Map<IndicatorResponseDTO>(indicator2),
                                      StockGroup = _mapper.Map<StockGroupResponse1DTO>(stockGroup)
                                  };

            var indicatorDraftsList = await indicatorDraftsQuery.ToListAsync();

            return ServiceResponse<List<IndicatorDraftResponseDto>>.Success(indicatorDraftsList);
        }

        public async Task<ServiceResponse<List<IndicatorDraftResponseDto>>> getAllIndicatorDrafts(int stockGroupId)
        {
            var indicatorDraftsQuery = from indicatorDraft in _context.IndicatorDrafts
                                       join stockGroup in _context.StockGroups on indicatorDraft.StockGroupId equals stockGroup.Id
                                       join indicator1 in _context.Indicators on indicatorDraft.IndicatorId1 equals indicator1.Id
                                       join indicator2 in _context.Indicators on indicatorDraft.IndicatorId2 equals indicator2.Id
                                       where indicatorDraft.StockGroupId == stockGroupId
                                       select new IndicatorDraftResponseDto
                                       {
                                           Id = indicatorDraft.Id,
                                           StockGroupId = indicatorDraft.StockGroupId,
                                           IndicatorId1 = indicatorDraft.IndicatorId1,
                                           IndicatorId2 = indicatorDraft.IndicatorId2,
                                           Type = indicatorDraft.Type,
                                           IndicatorId1Navigation = _mapper.Map<IndicatorResponseDTO>(indicator1),
                                           IndicatorId2Navigation = _mapper.Map<IndicatorResponseDTO>(indicator2),
                                           StockGroup = _mapper.Map<StockGroupResponse1DTO>(stockGroup)
                                       };

            var indicatorDraftsList = await indicatorDraftsQuery.ToListAsync();

            return ServiceResponse<List<IndicatorDraftResponseDto>>.Success(indicatorDraftsList);
        }
    }
}
