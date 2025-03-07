using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using VisssStock.Application.Interfaces;

namespace VisssStock.Application.Services.ConditonGroupService
{
    public class ConditionGroupService : IConditionGroupService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConditionGroupService(DataContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<ConditionGroupResponseDto>> createConditionGroup(ConditionGroupRequestDto conditionGroupRequestDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var conditionGroup = _mapper.Map<ConditionGroup>(conditionGroupRequestDto);
                conditionGroup.CreateBy = userId;
                conditionGroup.CreateDate = DateTime.Now;

                await _context.ConditionGroups.AddAsync(conditionGroup);

                await _context.SaveChangesAsync();

                return ServiceResponse<ConditionGroupResponseDto>.Success(_mapper.Map<ConditionGroupResponseDto>(conditionGroup));
            }
            catch(Exception ex)
            {
                return ServiceResponse<ConditionGroupResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<ConditionGroupResponseDto>> deleteConditionGroup(int id)
        {
            try
            {
                var userId = GetCurrentUserId();

                var conditionGroup = await _context.ConditionGroups
                    .Include(c => c.StockGroups)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (conditionGroup.CreateBy != userId)
                {
                    return ServiceResponse<ConditionGroupResponseDto>.Failure(403, "You are not allowed to delete this Condition Group");
                }

                conditionGroup.IsDeleted = 1;
                conditionGroup.UpdateBy = userId;
                conditionGroup.UpdateDate = DateTime.Now;

                if(conditionGroup.StockGroups != null && conditionGroup.StockGroups.Count() > 0)
                {
                    foreach (var stockGr in conditionGroup.StockGroups)
                    {
                        stockGr.IsDeleted = 1;
                        stockGr.UpdateBy = userId;
                        stockGr.UpdateDate = DateTime.Now;

                        _context.StockGroups.Update(stockGr);
                    }
                }
                
                if (conditionGroup == null)
                {
                    return ServiceResponse<ConditionGroupResponseDto>.Failure(404, "ConditionGroup not found");
                }

                _context.ConditionGroups.Update(conditionGroup);

                await _context.SaveChangesAsync();

                return ServiceResponse<ConditionGroupResponseDto>.Success(_mapper.Map<ConditionGroupResponseDto>(null));
            }
            catch (Exception ex)
            {
                return ServiceResponse<ConditionGroupResponseDto>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<ConditionGroupPagedListResponseDto>> getAllConditionGroups(OwnerParameters ownerParameters, ConditionGroupFillterDto requestDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var user = await _context.UserRoles
                    .Include(c => c.Role)
                    .Where(c => c.UserId == userId)
                    .ToListAsync(); 

                var conditionGroups = await _context.ConditionGroups
                    .Include(c => c.StockGroups.Where(c => c.IsDeleted == 0 && c.CreateBy == userId))
                    .Where(c => c.CreateBy == userId && c.IsDeleted == 0)
                    .ToListAsync();

                if (user.Any(c => c.Role.Name == "ADMIN"))
                {
                    conditionGroups = await _context.ConditionGroups
                    .Include(c => c.StockGroups.Where(c => c.IsDeleted == 0 && c.CreateBy == userId))
                    .Where(c => c.IsDeleted == 0)
                    .ToListAsync();
                }

                if (!string.IsNullOrEmpty(requestDto.Keyword))
                {
                    conditionGroups = conditionGroups
                        .Where(c => c.Name.ToUpper().Contains(requestDto.Keyword.ToUpper()))
                        .ToList();
                }

                var conditionGroupResponseDtos = _mapper.Map<List<ConditionGroupResponsexDto>>(conditionGroups);

                var pagedConditionGroups = conditionGroupResponseDtos
                    .Skip((ownerParameters.pageIndex - 1) * ownerParameters.pageSize)
                    .Take(ownerParameters.pageSize)
                    .ToList();

                var conditionGroupPagedListResponseDto = new ConditionGroupPagedListResponseDto
                {
                    ConditionGroups = pagedConditionGroups,
                    PageSize = ownerParameters.pageSize,
                    CurrentPage = ownerParameters.pageIndex,
                    TotalCount = conditionGroupResponseDtos.Count,
                    TotalPages = (int)Math.Ceiling(conditionGroupResponseDtos.Count / (double)ownerParameters.pageSize)
                };

                return ServiceResponse<ConditionGroupPagedListResponseDto>.Success(conditionGroupPagedListResponseDto);
            }
            catch (Exception ex)
            {
                return ServiceResponse<ConditionGroupPagedListResponseDto>.Failure(500, ex.Message);
            }
        }


        public async Task<ServiceResponse<ConditionGroupResponseDto>> updateConditionGroup(int id, ConditionGroupRequestDto conditionGroupRequestDto)
        {
            try
            {
                var userId = GetCurrentUserId();

                var conditionGroup = await _context.ConditionGroups
                    .FirstOrDefaultAsync(c => c.Id == id);

                if(conditionGroup.CreateBy != userId)
                {
                    return ServiceResponse<ConditionGroupResponseDto>.Failure(403, "You are not allowed to update this Condition Group");
                }

                conditionGroup.UpdateBy = userId;
                conditionGroup.UpdateDate = DateTime.Now;

                if (conditionGroup == null)
                {
                    return ServiceResponse<ConditionGroupResponseDto>.Failure(404, "ConditionGroup not found");
                }

                _mapper.Map(conditionGroupRequestDto, conditionGroup);

                _context.ConditionGroups.Update(conditionGroup);
                await _context.SaveChangesAsync();

                return ServiceResponse<ConditionGroupResponseDto>.Success(_mapper.Map<ConditionGroupResponseDto>(conditionGroup));
            }
            catch (Exception ex)
            {
                return ServiceResponse<ConditionGroupResponseDto>.Failure(500, ex.Message);
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
