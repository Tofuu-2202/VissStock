using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Interfaces;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Services.ScreenerServices
{
    public class ScreenerService : IScreenerService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ScreenerService> _logger;

        public ScreenerService(DataContext dataContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<ScreenerService> logger)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ServiceResponse<PagedListResponseDTO<ScreenerResponseDTO>>> GetAllScreenerAsync(OwnerParameters ownerParameters)
        {
            try
            {
                var screeners = await _dataContext.Screeners.Where(x => x.IsDeleted == 0).ToListAsync();
                var screenerResponseDTOs = _mapper.Map<List<ScreenerResponseDTO>>(screeners);

                // Áp dụng phân trang với PagedList
                var screenersPagedList = PagedList<ScreenerResponseDTO>.ToPagedList(screenerResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);
                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<ScreenerResponseDTO>(screenersPagedList);
                return ServiceResponse<PagedListResponseDTO<ScreenerResponseDTO>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<ScreenerResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<ScreenerResponseDTO>> GetScreenerByIdAsync(int screenerId)
        {
            try
            {
                var screener = await _dataContext.Screeners
                    .FirstOrDefaultAsync(s => s.Id == screenerId && s.IsDeleted == 0);

                if (screener == null)
                {
                    return ServiceResponse<ScreenerResponseDTO>.Failure(404, "Screener not found.");
                }

                var screenerResponseDTO = _mapper.Map<ScreenerResponseDTO>(screener);
                return ServiceResponse<ScreenerResponseDTO>.Success(screenerResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<ScreenerResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<ScreenerResponseDTO>> CreateScreenerAsync(ScreenerRequestDTO screenerRequestDTO)
        {
            try
            {
                // Lấy user id
                var userId = GetCurrentUserId();

                var screener = _mapper.Map<Screener>(screenerRequestDTO);
                screener.CreateBy = userId;
                screener.UpdateBy = userId;
                screener.CreateDate = DateTime.Now;
                screener.UpdateDate = DateTime.Now;
                screener.IsDeleted = 0;

                await _dataContext.Screeners.AddAsync(screener);
                await _dataContext.SaveChangesAsync();

                var screenerResponseDTO = _mapper.Map<ScreenerResponseDTO>(screener);
                return ServiceResponse<ScreenerResponseDTO>.Success(screenerResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<ScreenerResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<ScreenerResponseDTO>> UpdateScreenerByIdAsync(int screenerId, ScreenerRequestDTO screenerRequestDTO)
        {
            try
            {
                // Lấy user id
                var userId = GetCurrentUserId();

                var screener = await _dataContext.Screeners.FirstOrDefaultAsync(s => s.Id == screenerId && s.IsDeleted == 0);
                if (screener == null)
                {
                    return ServiceResponse<ScreenerResponseDTO>.Failure(404, "Screener not found.");
                }

                _mapper.Map(screenerRequestDTO, screener);
                screener.UpdateBy = userId;
                screener.UpdateDate = DateTime.Now;

                _dataContext.Screeners.Update(screener);
                await _dataContext.SaveChangesAsync();

                var screenerResponseDTO = _mapper.Map<ScreenerResponseDTO>(screener);
                return ServiceResponse<ScreenerResponseDTO>.Success(screenerResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<ScreenerResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<ScreenerResponseDTO>> DeleteScreenerByIdAsync(int screenerId)
        {
            try
            {
                var screener = await _dataContext.Screeners.FirstOrDefaultAsync(s => s.Id == screenerId && s.IsDeleted == 0);
                if (screener == null)
                {
                    return ServiceResponse<ScreenerResponseDTO>.Failure(404, "Screener not found.");
                }

                screener.IsDeleted = 1;
                _dataContext.Screeners.Update(screener);
                await _dataContext.SaveChangesAsync();

                var screenerResponseDTO = _mapper.Map<ScreenerResponseDTO>(screener);
                return ServiceResponse<ScreenerResponseDTO>.Success(screenerResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<ScreenerResponseDTO>.Failure(500, ex.Message);
            }
        }

        // Phương thức lấy userId từ HttpContext
        private int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
    }
}