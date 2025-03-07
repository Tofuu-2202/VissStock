using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using NCalc;
using ServiceStack;
using System.Text.RegularExpressions;
using VisssStock.Application.Interfaces;
namespace VisssStock.Application.Services.StockGroupIndicatorServices
{
    public class StockGroupIndicatorServiceImpl : IStockGroupIndicatorService
    {
        public DataContext _context;
        public IMapper _mapper;
        public IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StockGroupIndicatorServiceImpl(DataContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<StockGroupIndicatorResponseDTO>> deleteStockGroupIndicator(int id)
        {
            var stockGroupIndicator = await _context.StockGroupIndicators.FirstOrDefaultAsync(x => x.Id == id);
            if (stockGroupIndicator == null)
            {
                return new ServiceResponse<StockGroupIndicatorResponseDTO>
                {
                    Data = null,
                    Status = false,
                    Message = "StockGroupIndicator not found",
                    ErrorCode = 400
                };
            }
            //_context.StockGroupIndicators.Remove(stockGroupIndicator);

            stockGroupIndicator.IsDeleted = 1;
            stockGroupIndicator.UpdateDate = DateTime.Now;

            _context.StockGroupIndicators.Update(stockGroupIndicator);

            await _context.SaveChangesAsync();
            return ServiceResponse<StockGroupIndicatorResponseDTO>.Success(_mapper.Map<StockGroupIndicatorResponseDTO>(stockGroupIndicator));
        }

        public async Task<ServiceResponse<PagedListResponseDTO<StockGroupIndicatorResponseDTO>>> getAllStockGroupIndicators(OwnerParameters ownerParameters, string searchByName)
        {
            try
            {

                var userId = GetCurrentUserId();

                var stockGroupIndicator = await _context.StockGroupIndicators
                    .Include(x => x.Indicator)
                    .Include(x => x.User)
                    .Include(x => x.StockGroup)
                    .ThenInclude(s => s.StockGroupStocks)
                    .Where(x => x.IsDeleted == 0 && x.CreateBy == userId)
                    .ToListAsync();

                var stockGroupIndicatorDTO = _mapper.Map<List<StockGroupIndicatorResponseDTO>>(stockGroupIndicator);

                // Áp dụng phân trang với PagedList
                var stockGroupIndicatorsPagedList = PagedList<StockGroupIndicatorResponseDTO>.ToPagedList(stockGroupIndicatorDTO, ownerParameters.pageIndex, ownerParameters.pageSize);

                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<StockGroupIndicatorResponseDTO>(stockGroupIndicatorsPagedList);
                return ServiceResponse<PagedListResponseDTO<StockGroupIndicatorResponseDTO>>.Success(pagedResponse);

            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<StockGroupIndicatorResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupIndicatorResponseDTO>> getStockGroupIndicatorById(int id)
        {
            var userId = GetCurrentUserId();

            var stockGroupIndicator = await _context.StockGroupIndicators
                .Include(c => c)
                .FirstOrDefaultAsync(x => x.Id == id && x.CreateBy == userId);
            if (stockGroupIndicator == null)
            {
                return ServiceResponse<StockGroupIndicatorResponseDTO>.Failure(404, "StockGroupIndicator not found.");
            }
            return ServiceResponse<StockGroupIndicatorResponseDTO>.Success(_mapper.Map<StockGroupIndicatorResponseDTO>(stockGroupIndicator));
        }

        public async Task<ServiceResponse<StockGroupIndicatorResponseDTO>> updateStockGroupIndicator(UpdateStockGroupIndicatorDTO requestDto, int id)
        {
            var userId = GetCurrentUserId();

            var StockGroupExist = await _context.StockGroups.FirstOrDefaultAsync(x => x.Id == requestDto.StockGroupId);
            if (StockGroupExist == null)
            {
                return ServiceResponse<StockGroupIndicatorResponseDTO>.Failure(404, "Stock Group not found.");
            }

            var IndicatorExist = await _context.Indicators.FirstOrDefaultAsync(x => x.Id == requestDto.IndicatorId);

            if (IndicatorExist == null)
            {
                return ServiceResponse<StockGroupIndicatorResponseDTO>.Failure(404, "Indicator not found.");
            }

            var IntervalExist = await _context.Intervals.FirstOrDefaultAsync(x => x.Id == requestDto.IntervalId);
            if (IntervalExist == null)
            {
                return ServiceResponse<StockGroupIndicatorResponseDTO>.Failure(404, "Interval not found.");
            }

            var stockGroupIndicator = await _context.StockGroupIndicators.FirstOrDefaultAsync(x => x.Id == id);
            if (stockGroupIndicator == null)
            {
                return ServiceResponse<StockGroupIndicatorResponseDTO>.Failure(404, "StockGroupIndicator not found.");
            }

            stockGroupIndicator = _mapper.Map(requestDto, stockGroupIndicator);
            stockGroupIndicator.UpdateDate = DateTime.Now;
            stockGroupIndicator.UpdateBy = userId;

            await _context.SaveChangesAsync();

            return ServiceResponse<StockGroupIndicatorResponseDTO>.Success(_mapper.Map<StockGroupIndicatorResponseDTO>(stockGroupIndicator));
        }

        // Another Method:
        public async Task<ServiceResponse<List<StockGroupIndicatorResponseDTO>>> CreateUpdateStockGroupIndicatorAsync(int stockGroupId, List<StockGroupIndicatorRequestDTO> stockGroupIndicatorRequestDTOs)
        {
            try
            {
                var userId = GetCurrentUserId();
                var stockGroup = await _context.StockGroups.FirstOrDefaultAsync(sg => sg.Id == stockGroupId && sg.IsDeleted == 0 && sg.CreateBy == userId);

                if (stockGroup == null)
                {
                    return ServiceResponse<List<StockGroupIndicatorResponseDTO>>.Failure(404, "StockGroup not found or You dont have permission to edit this Stock Group");
                }

                // B1: Kiểm tra công thức trước khi thêm
                var validatedStockGroupIndicators = new List<StockGroupIndicator>();
                foreach (var dto in stockGroupIndicatorRequestDTOs)
                {
                    var formulaCheckResult = CheckFormula(dto.Formula);
                    if (formulaCheckResult.Status == false)
                    {
                        return ServiceResponse<List<StockGroupIndicatorResponseDTO>>.Failure(formulaCheckResult.ErrorCode, "Invalid Formula. Detail: " + formulaCheckResult.Message);
                    }

                    var stockGroupIndicator = _mapper.Map<StockGroupIndicator>(dto);
                    stockGroupIndicator.StockGroupId = stockGroupId;
                    stockGroupIndicator.CreateDate = DateTime.Now;
                    stockGroupIndicator.UpdateDate = DateTime.Now;
                    stockGroupIndicator.CreateBy = GetCurrentUserId();
                    stockGroupIndicator.UpdateBy = GetCurrentUserId();
                    stockGroupIndicator.IsDeleted = 0;
                    stockGroupIndicator.UserId = GetCurrentUserId();
                    validatedStockGroupIndicators.Add(stockGroupIndicator);
                }

                // B2: Xóa cứng tất cả các bản ghi của StockGroupIndicator có stockGroupId như stockGroupId truyền vào
                var existingStockGroupIndicators = await _context.StockGroupIndicators
                    .Where(sgi => sgi.StockGroupId == stockGroupId)
                    .ToListAsync();
                _context.StockGroupIndicators.RemoveRange(existingStockGroupIndicators);
                await _context.SaveChangesAsync();



                // B3: Thêm list bản ghi cho bảng StockGroupIndicator từ danh sách DTO
                await _context.StockGroupIndicators.AddRangeAsync(validatedStockGroupIndicators);
                await _context.SaveChangesAsync();

                // B4: Trả về List<StockGroupIndicatorResponseDTO> đã được cập nhật vào database
                var stockGroupIndicatorResponseDTOs = _mapper.Map<List<StockGroupIndicatorResponseDTO>>(validatedStockGroupIndicators);
                return ServiceResponse<List<StockGroupIndicatorResponseDTO>>.Success(stockGroupIndicatorResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockGroupIndicatorResponseDTO>>.Failure(500, ex.Message);
            }
        }

        // Hàm CheckFormula giữ nguyên như bạn đã viết

        public ServiceResponse<bool> CheckFormula(string expression)
        {
            try
            {
                var checkExpression = expression.Replace("stock", "1");
                var exp = new Expression(checkExpression);
                var result = exp.Evaluate();
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                var serviceRes = new ServiceResponse<bool>();
                serviceRes.Message = ex.Message;
                serviceRes.Status = false;
                serviceRes.ErrorCode = 400;
                serviceRes.Data = false;
                return serviceRes;
            }
        }

        public async Task<ServiceResponse<List<StockGroupIndicatorResponseDTO2>>> GetAllStockGroupIndicatorByStockGroupId(int stockGroupId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var user = await _context.Users
                    .Include(c => c.UserRoles)
                        .ThenInclude(c => c.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                // Lấy danh sách StockGroupIndicator dựa vào StockGroupId từ bảng StockGroupIndicator
                var stockGroupIndicators1 = _context.StockGroupIndicators
                    .Include(sgi => sgi.Indicator)
                    .Include(sgi => sgi.StockGroup)
                    .Include(sgi => sgi.User)
                    .Where(sgi => sgi.StockGroupId == stockGroupId && sgi.IsDeleted == 0 && sgi.CreateBy == userId)
                    .AsQueryable();

                if (user.UserRoles.Any(c => c.Role.Name == "ADMIN"))
                {
                    stockGroupIndicators1 = _context.StockGroupIndicators
                    .Include(sgi => sgi.Indicator)
                    .Include(sgi => sgi.StockGroup)
                    .Include(sgi => sgi.User)
                    .Where(sgi => sgi.StockGroupId == stockGroupId && sgi.IsDeleted == 0)
                    .AsQueryable();
                }

                var stockGroupIndicators = await stockGroupIndicators1.ToListAsync();

                // Map danh sách StockGroupIndicator sang StockGroupIndicatorResponseDTO2
                var stockGroupIndicatorResponseDTOs = _mapper.Map<List<StockGroupIndicatorResponseDTO2>>(stockGroupIndicators);

                // Trả về kết quả
                return ServiceResponse<List<StockGroupIndicatorResponseDTO2>>.Success(stockGroupIndicatorResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockGroupIndicatorResponseDTO2>>.Failure(500, ex.Message);
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
