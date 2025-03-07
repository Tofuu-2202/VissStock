using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using VisssStock.Application.Interfaces;

namespace VisssStock.Application.Services.StockServices
{
    public class StockService : IStockService
    {
        public DataContext _dataContext;
        public IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;

        public StockService(DataContext dataContext, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<PagedList<StockResponseDTO>>> GetAllStockAsync(OwnerParameters ownerParameters)
        {
            try
            {
                var stocks = await _dataContext.Stocks
                    .Include(x => x.Exchange)
                    .Include(x => x.Type)
                    .Include(x => x.Screener)
                    .Where(x => x.IsDeleted == 0).ToListAsync();
                var stockResponseDTOs = _mapper.Map<List<StockResponseDTO>>(stocks);
                return ServiceResponse<PagedList<StockResponseDTO>>.Success(PagedList<StockResponseDTO>.ToPagedList(stockResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize));
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedList<StockResponseDTO>>.Failure(500, ex.Message);
            }
        }


        public async Task<ServiceResponse<PagedListResponseDTO<StockResponseDTO>>> GetAllStockAsync(OwnerParameters ownerParameters, string name)
        {
            try
            {
                // 1. Tạo IQueryable để truy vấn dữ liệu từ database
                var stocksQuery = _dataContext.Stocks
                    .Include(x => x.Exchange)
                    .Include(x => x.Type)
                    .Include(x => x.Screener)
                    .Where(x => x.IsDeleted == 0);
                // 2. Áp dụng bộ lọc nếu có name và/hoặc description
                if (!string.IsNullOrEmpty(name))
                {
                    stocksQuery = stocksQuery.Where(s => s.Symbol.ToLower().Contains(name.ToLower()) || s.Description.ToLower().Contains(name.ToLower()));
                }
                // 3. Áp dụng phân trang với PagedList
                var stocks = await stocksQuery.ToListAsync();
                // 4. Map kết quả sang DTO và trả về
                var stockResponseDTOs = _mapper.Map<List<StockResponseDTO>>(stocks);
                var stocksPagedList = PagedList<StockResponseDTO>.ToPagedList(stockResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);
                var stocksPagedListResponse = new PagedListResponseDTO<StockResponseDTO>(stocksPagedList);
                return ServiceResponse<PagedListResponseDTO<StockResponseDTO>>.Success(stocksPagedListResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<StockResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<List<StockResponseDTO>>> GetAllStockAsyncForStockGroup(string name)
        {
            try
            {
                // 1. Tạo IQueryable để truy vấn dữ liệu từ database
                var stocksQuery = _dataContext.Stocks
                    .Where(x => x.IsDeleted == 0);
                // 2. Áp dụng bộ lọc nếu có name và/hoặc description
                if (!string.IsNullOrEmpty(name))
                {
                    stocksQuery = stocksQuery.Where(s => s.Symbol.ToLower().Contains(name.ToLower()) || s.Description.ToLower().Contains(name.ToLower()));
                }
                // 3. Áp dụng phân trang với PagedList
                var stocks = await stocksQuery.ToListAsync();
                // 4. Map kết quả sang DTO và trả về
                var stockResponseDTOs = _mapper.Map<List<StockResponseDTO>>(stocks);
                return ServiceResponse<List<StockResponseDTO>>.Success(stockResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockResponseDTO>> GetOneStockByIdAsync(int stockId)
        {
            try
            {
                var stock = await _dataContext.Stocks
                    .Include(x => x.Exchange)
                    .Include(x => x.Type)
                    .Include(x => x.Screener)
                    .FirstOrDefaultAsync(s => s.Id == stockId && s.IsDeleted == 0);

                if (stock == null)
                {
                    return ServiceResponse<StockResponseDTO>.Failure(404, "Stock not found.");
                }

                var stockResponseDTO = _mapper.Map<StockResponseDTO>(stock);
                return ServiceResponse<StockResponseDTO>.Success(stockResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockResponseDTO>> CreateStockAsync(StockRequestDTO stockRequestDTO)
        {
            try
            {
                // Kiểm tra TypeId và ExchangeId
                if (!await _dataContext.Types.AnyAsync(t => t.Id == stockRequestDTO.TypeId && t.IsDeleted == 0))
                {
                    return ServiceResponse<StockResponseDTO>.Failure(400, "Invalid TypeId.");
                }

                if (!await _dataContext.Exchanges.AnyAsync(e => e.Id == stockRequestDTO.ExchangeId && e.IsDeleted == 0))
                {
                    return ServiceResponse<StockResponseDTO>.Failure(400, "Invalid ExchangeId.");
                }

                if (!await _dataContext.Screeners.AnyAsync(s => s.Id == stockRequestDTO.ScreenerId && s.IsDeleted == 0))
                {
                    return ServiceResponse<StockResponseDTO>.Failure(400, "Invalid ScreenerId.");
                }

                // Lấy user id
                var userId = GetCurrentUserId();

                // Tạo object để save trên server
                var stock = _mapper.Map<Stock>(stockRequestDTO);
                stock.CreateBy = userId;
                stock.UpdateBy = userId;
                stock.CreateDate = DateTime.Now;
                stock.UpdateDate = DateTime.Now;
                stock.IsDeleted = 0;

                // save và lưu vào database
                await _dataContext.Stocks.AddAsync(stock);
                await _dataContext.SaveChangesAsync();

                // chuyển về dạng response trả lại cho người dùng xem
                var stockResponseDTO = _mapper.Map<StockResponseDTO>(stock);
                return ServiceResponse<StockResponseDTO>.Success(stockResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockResponseDTO>> UpdateStockByIdAsync(int stockId, StockRequestDTO stockRequestDTO)
        {
            try
            {
                // Kiểm tra TypeId và ExchangeId
                if (!await _dataContext.Types.AnyAsync(t => t.Id == stockRequestDTO.TypeId && t.IsDeleted == 0))
                {
                    return ServiceResponse<StockResponseDTO>.Failure(400, "Invalid TypeId.");
                }

                if (!await _dataContext.Exchanges.AnyAsync(e => e.Id == stockRequestDTO.ExchangeId && e.IsDeleted == 0))
                {
                    return ServiceResponse<StockResponseDTO>.Failure(400, "Invalid ExchangeId.");
                }
                if (!await _dataContext.Screeners.AnyAsync(s => s.Id == stockRequestDTO.ScreenerId && s.IsDeleted == 0))
                {
                    return ServiceResponse<StockResponseDTO>.Failure(400, "Invalid ScreenerId.");
                }

                // Lấy user id
                var userId = GetCurrentUserId();

                // Tạo object để save trên server
                var stock = await _dataContext.Stocks.FirstOrDefaultAsync(e => e.Id == stockId && e.IsDeleted == 0);
                if (stock == null)
                {
                    return ServiceResponse<StockResponseDTO>.Failure(404, "Stock not found.");
                }

                // Update trường dữ liệu
                _mapper.Map(stockRequestDTO, stock);
                stock.UpdateBy = userId;
                stock.UpdateDate = DateTime.Now;

                // save và lưu vào database
                _dataContext.Stocks.Update(stock);
                await _dataContext.SaveChangesAsync();

                // chuyển về dạng response trả lại cho người dùng xem
                var stockResponseDTO = _mapper.Map<StockResponseDTO>(stock);
                return ServiceResponse<StockResponseDTO>.Success(stockResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockResponseDTO>> DeleteStockByIdAsync(int stockId)
        {
            try
            {
                var stock = await _dataContext.Stocks.FirstOrDefaultAsync(e => e.Id == stockId && e.IsDeleted == 0);
                if (stock == null)
                {
                    return ServiceResponse<StockResponseDTO>.Failure(404, "Stock not found.");
                }

                stock.IsDeleted = 1;
                var stockResponseDto = _mapper.Map<StockResponseDTO>(stock);
                _dataContext.Stocks.Update(stock);
                await _dataContext.SaveChangesAsync();

                return ServiceResponse<StockResponseDTO>.Success(stockResponseDto);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockResponseDTO>.Failure(500, ex.Message);
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