using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System.Security.Claims;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Infrastructure.Data;
using VisssStock.Application.Interfaces;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Services.StockGroupStockServices
{
    public class StockGroupStockService : IStockGroupStockService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StockGroupStockService(DataContext dataContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<PagedListResponseDTO<StockGroupStockResponseDTO>>> GetAllStockGroupStockAsync(OwnerParameters ownerParameters)
        {
            try
            {
                // Sử dụng LINQ Join để lấy dữ liệu cần thiết
                var query = from sgs in _dataContext.StockGroupStocks
                            join s in _dataContext.Stocks on sgs.StockId equals s.Id
                            join sg in _dataContext.StockGroups on sgs.StockGroupId equals sg.Id
                            join e in _dataContext.Exchanges on s.ExchangeId equals e.Id
                            join t in _dataContext.Types on s.TypeId equals t.Id
                            join scr in _dataContext.Screeners on s.ScreenerId equals scr.Id
                            where sgs.IsDeleted == 0 && s.IsDeleted == 0 && sg.IsDeleted == 0
                                  && e.IsDeleted == 0 && t.IsDeleted == 0 && scr.IsDeleted == 0
                            orderby sgs.IsLike descending
                            select new
                            {
                                StockGroupStock = sgs,
                                Stock = s,
                                StockGroup = sg,
                                Exchange = e,
                                Type = t,
                                Screener = scr
                            };

                var result = await query.ToListAsync();

                // Map the result to StockGroupStockResponseDTO using AutoMapper
                var stockGroupStockResponseDTOs = result.Select(item =>
                {
                    var stockGroupStockDto = _mapper.Map<StockGroupStockResponseDTO>(item.StockGroupStock);
                    stockGroupStockDto.Stock = _mapper.Map<StockResponseDTO>(item.Stock);
                    stockGroupStockDto.Stock.ExchangeResponse = _mapper.Map<ExchangeResponseDTO>(item.Exchange);
                    stockGroupStockDto.Stock.TypeResponse = _mapper.Map<TypeResponseDTO>(item.Type);
                    stockGroupStockDto.Stock.ScreenerResponse = _mapper.Map<ScreenerResponseDTO>(item.Screener);
                    stockGroupStockDto.StockGroup = _mapper.Map<StockGroupResponseDTO>(item.StockGroup);
                    return stockGroupStockDto;
                }).ToList();

                // Áp dụng phân trang với PagedList
                var stockGroupStocksPagedList = PagedList<StockGroupStockResponseDTO>.ToPagedList(stockGroupStockResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);

                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<StockGroupStockResponseDTO>(stockGroupStocksPagedList);
                return ServiceResponse<PagedListResponseDTO<StockGroupStockResponseDTO>>.Success(pagedResponse);

            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<StockGroupStockResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupStockResponseDTO>> GetStockGroupStockByIdAsync(int stockGroupStockId)
        {
            try
            {
                // Sử dụng LINQ Join để lấy dữ liệu cần thiết
                var query = from sgs in _dataContext.StockGroupStocks
                            join s in _dataContext.Stocks on sgs.StockId equals s.Id
                            join sg in _dataContext.StockGroups on sgs.StockGroupId equals sg.Id
                            join e in _dataContext.Exchanges on s.ExchangeId equals e.Id
                            join t in _dataContext.Types on s.TypeId equals t.Id
                            join scr in _dataContext.Screeners on s.ScreenerId equals scr.Id
                            where sgs.Id == stockGroupStockId &&
                                  sgs.IsDeleted == 0 &&
                                  s.IsDeleted == 0 &&
                                  sg.IsDeleted == 0 &&
                                  e.IsDeleted == 0 &&
                                  t.IsDeleted == 0 &&
                                  scr.IsDeleted == 0
                            select new
                            {
                                StockGroupStock = sgs,
                                Stock = s,
                                StockGroup = sg,
                                Exchange = e,
                                Type = t,
                                Screener = scr
                            };

                // Lấy kết quả đầu tiên hoặc null nếu không tìm thấy
                var result = await query.FirstOrDefaultAsync();

                // Kiểm tra kết quả
                if (result == null)
                {
                    return ServiceResponse<StockGroupStockResponseDTO>.Failure(404, "StockGroupStock not found.");
                }

                // Sử dụng AutoMapper để map dữ liệu sang DTO
                var stockGroupStockResponseDTO = _mapper.Map<StockGroupStockResponseDTO>(result.StockGroupStock);
                stockGroupStockResponseDTO.Stock = _mapper.Map<StockResponseDTO>(result.Stock);
                stockGroupStockResponseDTO.Stock.ExchangeResponse = _mapper.Map<ExchangeResponseDTO>(result.Exchange);
                stockGroupStockResponseDTO.Stock.TypeResponse = _mapper.Map<TypeResponseDTO>(result.Type);
                stockGroupStockResponseDTO.Stock.ScreenerResponse = _mapper.Map<ScreenerResponseDTO>(result.Screener);
                stockGroupStockResponseDTO.StockGroup = _mapper.Map<StockGroupResponseDTO>(result.StockGroup);

                // Trả về kết quả
                return ServiceResponse<StockGroupStockResponseDTO>.Success(stockGroupStockResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupStockResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<List<StockGroupStockResponseDTO>>> CreateStockGroupStockAsync(List<StockGroupStockRequestDTO> stockGroupStockRequestDTOs)
        {
            try
            {
                // Danh sách StockGroupStockResponseDTO để trả về
                var stockGroupStockResponseDTOs = new List<StockGroupStockResponseDTO>();

                // Lấy UserId
                var userId = GetCurrentUserId();

                // Duyệt qua danh sách StockGroupStockRequestDTO
                foreach (var stockGroupStockRequestDTO in stockGroupStockRequestDTOs)
                {
                    // Kiểm tra xem StockId và StockGroupId có tồn tại không
                    if (!await _dataContext.Stocks.AnyAsync(s => s.Id == stockGroupStockRequestDTO.StockId && s.IsDeleted == 0))
                    {
                        return ServiceResponse<List<StockGroupStockResponseDTO>>.Failure(400, $"Invalid StockId: {stockGroupStockRequestDTO.StockId}");
                    }

                    if (!await _dataContext.StockGroups.AnyAsync(sg => sg.Id == stockGroupStockRequestDTO.StockGroupId && sg.IsDeleted == 0))
                    {
                        return ServiceResponse<List<StockGroupStockResponseDTO>>.Failure(400, $"Invalid StockGroupId: {stockGroupStockRequestDTO.StockGroupId}");
                    }

                    // Tạo đối tượng StockGroupStock
                    var stockGroupStock = _mapper.Map<StockGroupStock>(stockGroupStockRequestDTO);
                    stockGroupStock.CreateDate = DateTime.Now;
                    stockGroupStock.UpdateDate = DateTime.Now;
                    stockGroupStock.CreateBy = userId;
                    stockGroupStock.UpdateBy = userId;
                    stockGroupStock.IsDeleted = 0;

                    // Thêm vào database
                    await _dataContext.StockGroupStocks.AddAsync(stockGroupStock);
                    await _dataContext.SaveChangesAsync();

                    // Thêm StockGroupStockResponseDTO vào danh sách
                    stockGroupStockResponseDTOs.Add(_mapper.Map<StockGroupStockResponseDTO>(stockGroupStock));
                }

                // Trả về danh sách StockGroupStockResponseDTO
                return ServiceResponse<List<StockGroupStockResponseDTO>>.Success(stockGroupStockResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockGroupStockResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupStockResponseDTO>> UpdateStockGroupStockByIdAsync(int stockGroupStockId, StockGroupStockRequestDTO stockGroupStockRequestDTO)
        {
            try
            {
                var stockGroupStock = await _dataContext.StockGroupStocks.FirstOrDefaultAsync(sgs => sgs.Id == stockGroupStockId && sgs.IsDeleted == 0);
                if (stockGroupStock == null)
                {
                    return ServiceResponse<StockGroupStockResponseDTO>.Failure(404, "StockGroupStock not found.");
                }

                // Kiểm tra xem StockId và StockGroupId mới có hợp lệ không
                if (!await _dataContext.Stocks.AnyAsync(s => s.Id == stockGroupStockRequestDTO.StockId && s.IsDeleted == 0))
                {
                    return ServiceResponse<StockGroupStockResponseDTO>.Failure(400, "Invalid StockId.");
                }

                if (!await _dataContext.StockGroups.AnyAsync(sg => sg.Id == stockGroupStockRequestDTO.StockGroupId && sg.IsDeleted == 0))
                {
                    return ServiceResponse<StockGroupStockResponseDTO>.Failure(400, "Invalid StockGroupId.");
                }

                // Lấy UserId
                var userId = GetCurrentUserId();

                // Cập nhật StockGroupStock
                _mapper.Map(stockGroupStockRequestDTO, stockGroupStock);
                stockGroupStock.UpdateDate = DateTime.Now;
                stockGroupStock.UpdateBy = userId;

                _dataContext.StockGroupStocks.Update(stockGroupStock);
                await _dataContext.SaveChangesAsync();

                var stockGroupStockResponseDTO = _mapper.Map<StockGroupStockResponseDTO>(stockGroupStock);
                return ServiceResponse<StockGroupStockResponseDTO>.Success(stockGroupStockResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupStockResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupStockResponseDTO>> DeleteStockGroupStockByIdAsync(int stockGroupStockId)
        {
            try
            {
                var stockGroupStock = await _dataContext.StockGroupStocks.FirstOrDefaultAsync(sgs => sgs.Id == stockGroupStockId && sgs.IsDeleted == 0);
                if (stockGroupStock == null)
                {
                    return ServiceResponse<StockGroupStockResponseDTO>.Failure(404, "StockGroupStock not found.");
                }

                stockGroupStock.IsDeleted = 1;
                _dataContext.StockGroupStocks.Update(stockGroupStock);
                await _dataContext.SaveChangesAsync();

                var stockGroupStockResponseDTO = _mapper.Map<StockGroupStockResponseDTO>(stockGroupStock);
                return ServiceResponse<StockGroupStockResponseDTO>.Success(stockGroupStockResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupStockResponseDTO>.Failure(500, ex.Message);
            }
        }

        // Another Method
        public async Task<ServiceResponse<List<StockGroupStockResponseDTO>>> CreateUpdateStockInStockGroupAsync(int stockGroupId, List<int> stockIds)
        {
            try
            {
                var userId = GetCurrentUserId();

                var stockGroup = await _dataContext.StockGroups.FirstOrDefaultAsync(sg => sg.Id == stockGroupId && sg.IsDeleted == 0 && sg.CreateBy == userId);

                if (stockGroup == null)
                {
                    return ServiceResponse<List<StockGroupStockResponseDTO>>.Failure(404, "StockGroup not found or You don't have permission to edit this stock Group");
                }

                // B1: Xóa mềm tất cả các bản ghi của StockGroupStock có stockGroupId như stockGroupId truyền vào
                var existingStockGroupStocks = await _dataContext.StockGroupStocks
                    .Where(sgs => sgs.StockGroupId == stockGroupId && sgs.IsDeleted == 0)
                    .ToListAsync();

                _dataContext.StockGroupStocks.RemoveRange(existingStockGroupStocks); // Update nhiều bản ghi cùng lúc
                await _dataContext.SaveChangesAsync();

                // B2: Thêm list bản ghi cho bảng StockGroupStock với các bản ghi là stockGroupId, stockId (từ list truyền vào)
                var newStockGroupStocks = stockIds.Select(stockId => new StockGroupStock
                {
                    StockId = stockId,
                    StockGroupId = stockGroupId,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    CreateBy = GetCurrentUserId(),
                    UpdateBy = GetCurrentUserId(),
                    IsDeleted = 0
                }).ToList();

                await _dataContext.StockGroupStocks.AddRangeAsync(newStockGroupStocks);
                await _dataContext.SaveChangesAsync();

                // B3: Trả về List<StockGroupStockResponseDTO> đã được cập nhật vào database
                var stockGroupStockResponseDTOs = _mapper.Map<List<StockGroupStockResponseDTO>>(newStockGroupStocks);
                return ServiceResponse<List<StockGroupStockResponseDTO>>.Success(stockGroupStockResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockGroupStockResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<List<StockGroupStockResponseDTO>>> AddToFavoriteStock(int stockGroupStockId, int isFavorite)
        {
            try
            {
                var existingStockGroupStocks = await _dataContext.StockGroupStocks
                    .Where(sgs => sgs.Id == stockGroupStockId && sgs.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                existingStockGroupStocks.IsLike = isFavorite;

                _dataContext.StockGroupStocks.Update(existingStockGroupStocks);

                await _dataContext.SaveChangesAsync();

                return ServiceResponse<List<StockGroupStockResponseDTO>>.Success(new List<StockGroupStockResponseDTO>());
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockGroupStockResponseDTO>>.Failure(500, ex.Message);
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
