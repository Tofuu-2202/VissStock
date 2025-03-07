using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Services.StockGroupServices
{
    public class StockGroupService : IStockGroupService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<StockGroupService> _logger;

        public StockGroupService(DataContext dataContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ILogger<StockGroupService> logger)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ServiceResponse<PagedListResponseDTO<StockGroupResponseDTO>>> GetAllStockGroupAsync(OwnerParameters ownerParameters, StockGroupFilterDto requestDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _dataContext.UserRoles
                    .Include(c => c.Role)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                // Áp dụng điều kiện trước khi gọi ToListAsync()
                var stockGroupsQuery = _dataContext.StockGroups
                    .Include(c => c.ConditionGroup)
                    .Include(c => c.Interval)
                    .Include(c => c.TelegramBot)
                    .Where(x => x.IsDeleted == 0 && x.CreateBy == userId);

                if (user.Any(c => c.Role.Name == "ADMIN"))
                {
                    stockGroupsQuery = _dataContext.StockGroups
                        .Include(c => c.ConditionGroup)
                        .Include(c => c.Interval)
                        .Include(c => c.TelegramBot)
                        .Where(x => x.IsDeleted == 0);
                }

                // Áp dụng các bộ lọc
                if (!string.IsNullOrEmpty(requestDto.Keyword))
                {
                    stockGroupsQuery = stockGroupsQuery.Where(x => x.Name.ToLower().Contains(requestDto.Keyword.ToLower()));
                }

                if (requestDto.IntervalId > 0)
                {
                    stockGroupsQuery = stockGroupsQuery.Where(x => x.IntervalId == requestDto.IntervalId);
                }

                if (requestDto.ConditionGroupId > 0)
                {
                    stockGroupsQuery = stockGroupsQuery.Where(x => x.ConditionGroupId == requestDto.ConditionGroupId);
                }

                if (requestDto.TelegramBotId > 0)
                {
                    stockGroupsQuery = stockGroupsQuery.Where(x => x.TelegramBotId == requestDto.TelegramBotId);
                }

                // Thực hiện truy vấn và lấy danh sách từ cơ sở dữ liệu
                var stockGroups = await stockGroupsQuery.ToListAsync();

                // Áp dụng sắp xếp
                var stockGroupResponseDTOs = _mapper.Map<List<StockGroupResponseDTO>>(stockGroups);
                stockGroupResponseDTOs = stockGroupResponseDTOs.OrderBy(c => c.ConditionGroupId)
                                                                .ThenBy(c => c.Name)
                                                                .ThenBy(c => c.IntervalId)
                                                                .ThenBy(c => c.TelegramBotId)
                                                                .ToList();

                // Áp dụng phân trang
                var pagedStockGroups = PagedList<StockGroupResponseDTO>.ToPagedList(stockGroupResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);

                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<StockGroupResponseDTO>(pagedStockGroups);
                return ServiceResponse<PagedListResponseDTO<StockGroupResponseDTO>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<StockGroupResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupResponseDTO>> GetStockGroupByIdAsync(int stockGroupId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var stockGroup = await _dataContext.StockGroups
                    .Include(c => c.ConditionGroup)
                    .Include(c => c.Interval)
                    .Include(c => c.TelegramBot)
                    .FirstOrDefaultAsync(sg => sg.Id == stockGroupId && sg.IsDeleted == 0 && sg.CreateBy == userId);

                if (stockGroup == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Stock group not found.");
                }

                var stockGroupResponseDTO = _mapper.Map<StockGroupResponseDTO>(stockGroup);
                return ServiceResponse<StockGroupResponseDTO>.Success(stockGroupResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupAsync(StockGroupRequestDTO stockGroupRequestDTO)
        {
            try
            {
                //check interval
                var interval = await _dataContext.Intervals.FirstOrDefaultAsync(i => i.Id == stockGroupRequestDTO.IntervalId);
                if (interval == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Interval not found.");
                }

                // Lấy user id
                var userId = GetCurrentUserId();
                var stockGroup = _mapper.Map<StockGroup>(stockGroupRequestDTO);
                if(stockGroupRequestDTO.ConditionGroupId == 0) stockGroup.ConditionGroupId = null;
                stockGroup.CreateBy = userId;
                stockGroup.UpdateBy = userId;
                stockGroup.CreateDate = DateTime.Now;
                stockGroup.UpdateDate = DateTime.Now;
                stockGroup.IsDeleted = 0;
                stockGroup.IsActive = 1;

                await _dataContext.StockGroups.AddAsync(stockGroup);
                await _dataContext.SaveChangesAsync();

                var stockGroupResponseDTO = _mapper.Map<StockGroupResponseDTO>(stockGroup);
                return ServiceResponse<StockGroupResponseDTO>.Success(stockGroupResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupResponseDTO>> UpdateStockGroupByIdAsync(int stockGroupId, StockGroupRequestDTO stockGroupRequestDTO)
        {
            try
            {
                //check interval
                var interval = await _dataContext.Intervals.FirstOrDefaultAsync(i => i.Id == stockGroupRequestDTO.IntervalId);
                if (interval == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Interval not found.");
                }

                // Lấy user id
                var userId = GetCurrentUserId();

                var stockGroup = await _dataContext.StockGroups.FirstOrDefaultAsync(sg => sg.Id == stockGroupId && sg.IsDeleted == 0);
                if (stockGroup == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Stock group not found.");
                }

                _mapper.Map(stockGroupRequestDTO, stockGroup);

                if(stockGroupRequestDTO.ConditionGroupId == 0) stockGroup.ConditionGroupId = null;
                stockGroup.UpdateBy = userId;
                stockGroup.UpdateDate = DateTime.Now;

                _dataContext.StockGroups.Update(stockGroup);
                await _dataContext.SaveChangesAsync();

                var stockGroupResponseDTO = _mapper.Map<StockGroupResponseDTO>(stockGroup);
                return ServiceResponse<StockGroupResponseDTO>.Success(stockGroupResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupResponseDTO>> UpdateStockGroupStatusByIdAsync(int stockGroupId)
        {
            try
            {
                // Lấy user id
                var userId = GetCurrentUserId();

                var stockGroup = await _dataContext.StockGroups.FirstOrDefaultAsync(sg => sg.Id == stockGroupId && sg.IsDeleted == 0);
                if (stockGroup == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Stock group not found.");
                }

                stockGroup.IsActive = stockGroup.IsActive == 0 ? 1 : 0;
                stockGroup.UpdateBy = userId;
                stockGroup.UpdateDate = DateTime.Now;

                _dataContext.StockGroups.Update(stockGroup);
                await _dataContext.SaveChangesAsync();

                var stockGroupResponseDTO = _mapper.Map<StockGroupResponseDTO>(stockGroup);
                return ServiceResponse<StockGroupResponseDTO>.Success(stockGroupResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupResponseDTO>> DeleteStockGroupByIdAsync(int stockGroupId)
        {
            try
            {
                var stockGroup = await _dataContext.StockGroups.FirstOrDefaultAsync(sg => sg.Id == stockGroupId && sg.IsDeleted == 0);
                if (stockGroup == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Stock group not found.");
                }

                stockGroup.IsDeleted = 1;
                _dataContext.StockGroups.Update(stockGroup);
                await _dataContext.SaveChangesAsync();

                var stockGroupResponseDTO = _mapper.Map<StockGroupResponseDTO>(stockGroup);
                return ServiceResponse<StockGroupResponseDTO>.Success(stockGroupResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }


        // Many to many với bảng Stock, sử dụng bảng trung gian StockGroupStock
        public async Task<ServiceResponse<PagedListResponseDTO<StockResponseDTO>>> GetAllStockByStockGroupIdAsync(OwnerParameters ownerParameters, int stockGroupId)
        {
            try
            {
                var stocks = await _dataContext.StockGroupStocks
                    .Include(sgs => sgs.Stock)
                    .ThenInclude(s => s.Exchange)
                    .Include(sgs => sgs.Stock)
                    .ThenInclude(s => s.Type)
                    .Include(sgs => sgs.Stock)
                    .ThenInclude(s => s.Screener)
                    .Where(sgs => sgs.StockGroupId == stockGroupId &&
                                  sgs.IsDeleted == 0 &&
                                  sgs.Stock.IsDeleted == 0)
                    .OrderByDescending(c => c.IsLike)
                    .Select(sgs => sgs.Stock)
                    .Where(c => c.IsDeleted == 0)
                    .ToListAsync();

                var stockResponseDTOs = _mapper.Map<List<StockResponseDTO>>(stocks);
                var pagedStocks = PagedList<StockResponseDTO>.ToPagedList(stockResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);

                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<StockResponseDTO>(pagedStocks);
                return ServiceResponse<PagedListResponseDTO<StockResponseDTO>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<StockResponseDTO>>.Failure(500, ex.Message);
            }
        }
        public async Task<ServiceResponse<StockGroupResponseDTO>> AddStockToStockGroupByStockGroupId(int stockId, int stockGroupId)
        {
            try
            {
                // Kiểm tra xem Stock và StockGroup có tồn tại hay không
                var stockExists = await _dataContext.Stocks.AnyAsync(s => s.Id == stockId && s.IsDeleted == 0);
                var stockGroupExists = await _dataContext.StockGroups.AnyAsync(sg => sg.Id == stockGroupId && sg.IsDeleted == 0);

                if (!stockExists || !stockGroupExists)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Stock or StockGroup not found.");
                }

                // Kiểm tra xem mối quan hệ đã tồn tại chưa
                var relationshipExists = await _dataContext.StockGroupStocks
                    .AnyAsync(sgs => sgs.StockId == stockId && sgs.StockGroupId == stockGroupId && sgs.IsDeleted == 0);

                if (relationshipExists)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(400, "Relationship already exists.");
                }

                // Lấy userId
                var userId = GetCurrentUserId();

                // Tạo StockGroupStock mới
                var stockGroupStock = new StockGroupStock
                {
                    StockId = stockId,
                    StockGroupId = stockGroupId,
                    CreateBy = userId,
                    CreateDate = DateTime.Now,
                    IsDeleted = 0
                };

                await _dataContext.StockGroupStocks.AddAsync(stockGroupStock);
                await _dataContext.SaveChangesAsync();

                // Lấy StockGroup để trả về
                var stockGroup = await _dataContext.StockGroups.FindAsync(stockGroupId);
                var stockGroupResponseDTO = _mapper.Map<StockGroupResponseDTO>(stockGroup);

                return ServiceResponse<StockGroupResponseDTO>.Success(stockGroupResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }
        public async Task<ServiceResponse<StockGroupResponseDTO>> RemoveStockInStockGroupByStockGroupId(int stockId, int stockGroupId)
        {
            try
            {
                var stockGroupStock = await _dataContext.StockGroupStocks
                    .FirstOrDefaultAsync(sgs => sgs.StockId == stockId && sgs.StockGroupId == stockGroupId && sgs.IsDeleted == 0);

                if (stockGroupStock == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Relationship not found.");
                }

                stockGroupStock.IsDeleted = 1;
                _dataContext.StockGroupStocks.Update(stockGroupStock);
                await _dataContext.SaveChangesAsync();

                var stockGroup = await _dataContext.StockGroups.FindAsync(stockGroupId);
                var stockGroupResponseDTO = _mapper.Map<StockGroupResponseDTO>(stockGroup);

                return ServiceResponse<StockGroupResponseDTO>.Success(stockGroupResponseDTO);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }


        // Phương thức lấy userId từ HttpContext
        private int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }

        //create groupstock by clone 
        public async Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneAsync(int stockGroupId)
        {
            try
            {
                var groupStock = await _dataContext.StockGroups
                    .Include(sg => sg.Interval)
                    .Include(sg => sg.StockGroupStocks)
                    .Include(c => c.StockGroupIndicators)
                    .Where(sg => sg.Id == stockGroupId && sg.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                var userId = GetCurrentUserId();

                var newGroupStock = new StockGroupRequestDTO
                {
                    IntervalId = groupStock.IntervalId,
                    Name = groupStock.Name + " Clone",
                    ConditionGroupId = groupStock.ConditionGroupId ?? null
                };

                var stockGroupResponse = await CreateStockGroupAsync(newGroupStock);

                if (!stockGroupResponse.Status)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(stockGroupResponse.ErrorCode, stockGroupResponse.Message);
                }

                var newStockGroupId = stockGroupResponse.Data.Id;

                if (groupStock.StockGroupStocks.Count > 0)
                {
                    var stockGroupStocks = new List<StockGroupStock>();
                    foreach (var stockGroupStock in groupStock.StockGroupStocks)
                    {
                        var newStockGroupStock = new StockGroupStock
                        {
                            StockGroupId = newStockGroupId,
                            StockId = stockGroupStock.StockId,
                            CreateBy = userId,
                            CreateDate = DateTime.Now,
                            IsDeleted = 0
                        };
                        stockGroupStocks.Add(newStockGroupStock);
                    }

                    _dataContext.StockGroupStocks.AddRange(stockGroupStocks);
                    await _dataContext.SaveChangesAsync();
                }

                if (groupStock.StockGroupIndicators.Count > 0)
                {
                    var stockGroupIndicators = new List<StockGroupIndicator>();
                    foreach (var stockGroupIndicator in groupStock.StockGroupIndicators)
                    {
                        var newStockGroupIndicator = new StockGroupIndicator
                        {
                            StockGroupId = newStockGroupId,
                            UserId = userId,
                            Formula = stockGroupIndicator.Formula,
                            IndicatorId = stockGroupIndicator.IndicatorId,
                            CreateBy = userId,
                            CreateDate = DateTime.Now,
                            IsDeleted = 0
                        };
                        stockGroupIndicators.Add(newStockGroupIndicator);
                    }

                    _dataContext.StockGroupIndicators.AddRange(stockGroupIndicators);
                    await _dataContext.SaveChangesAsync();
                }


                return ServiceResponse<StockGroupResponseDTO>.Success(stockGroupResponse.Data);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupResponseDTO>> ReplaceIndicatorOfStockGroupIntoOtherStockGroupAsync(int stockGroupIdInput, int stockGroupIdOutput)
        {
            try
            {
                var groupStockInput = await _dataContext.StockGroups
                    .Include(c => c.StockGroupIndicators)
                    .Where(sg => sg.Id == stockGroupIdInput && sg.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                if (groupStockInput == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Stock group Input not found.");
                }

                var userId = GetCurrentUserId();

                var groupStockOutput = await _dataContext.StockGroups
                    .Include(c => c.StockGroupIndicators)
                    .Where(sg => sg.Id == stockGroupIdOutput && sg.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                if (groupStockOutput == null)
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Stock group Output not found.");
                }
                
                if (groupStockInput.StockGroupIndicators.Count > 0)
                {
                    //remove all indicator of stock group output
                    foreach (var stockGroupIndicator in groupStockOutput.StockGroupIndicators)
                    {
                        stockGroupIndicator.UpdateBy = userId;
                        stockGroupIndicator.UpdateDate = DateTime.Now;
                        stockGroupIndicator.IsDeleted = 1;
                    }
                    _dataContext.StockGroupIndicators.UpdateRange(groupStockOutput.StockGroupIndicators);

                    var stockGroupIndicators = new List<StockGroupIndicator>();
                    foreach (var stockGroupIndicator in groupStockInput.StockGroupIndicators)
                    {
                        var newStockGroupIndicator = new StockGroupIndicator
                        {
                            StockGroup = groupStockOutput,
                            UserId = userId,
                            Formula = stockGroupIndicator.Formula,
                            IndicatorId = stockGroupIndicator.IndicatorId,
                            CreateBy = userId,
                            CreateDate = DateTime.Now,
                            IsDeleted = 0
                        };
                        stockGroupIndicators.Add(newStockGroupIndicator);
                    }

                    _dataContext.StockGroupIndicators.AddRange(stockGroupIndicators);
                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    return ServiceResponse<StockGroupResponseDTO>.Failure(404, "Indicator of Stock group Input not found.");
                }


                return ServiceResponse<StockGroupResponseDTO>.Success(null);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneByConditionGroupIdAsync(int conditionGroupId)
        {
            try
            {
                var groupStocks = await _dataContext.StockGroups
                    .Where(sg => sg.ConditionGroupId == conditionGroupId && sg.IsDeleted == 0)
                    .ToListAsync();

                var conditionGroupEx = await _dataContext.ConditionGroups
                    .Where(cg => cg.Id == conditionGroupId && cg.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                var userId = GetCurrentUserId();

                var newConditionGroup = new ConditionGroup
                {
                    Name = conditionGroupEx.Name + " Clone",
                    CreateBy = userId,
                    CreateDate = DateTime.Now,
                    IsDeleted = 0
                };

                _dataContext.ConditionGroups.Add(newConditionGroup);


                foreach(var stockGroupId in groupStocks.Select(c => c.Id))
                {
                    var groupStock = await _dataContext.StockGroups
                    .Include(sg => sg.Interval)
                    .Include(sg => sg.StockGroupStocks)
                    .Include(c => c.StockGroupIndicators)
                    .Where(sg => sg.Id == stockGroupId && sg.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                    var newGroupStock = new StockGroup
                    {
                        IntervalId = groupStock.IntervalId,
                        Name = groupStock.Name + " Clone",
                        ConditionGroup = newConditionGroup,
                        CreateBy = userId,
                        CreateDate = DateTime.Now,
                    };

                    _dataContext.StockGroups.Add(newGroupStock);

                    if (groupStock.StockGroupStocks.Count > 0)
                    {
                        var stockGroupStocks = new List<StockGroupStock>();
                        foreach (var stockGroupStock in groupStock.StockGroupStocks)
                        {
                            var newStockGroupStock = new StockGroupStock
                            {
                                StockGroup = newGroupStock,
                                StockId = stockGroupStock.StockId,
                                CreateBy = userId,
                                CreateDate = DateTime.Now,
                                IsDeleted = 0
                            };
                            stockGroupStocks.Add(newStockGroupStock);
                        }

                        _dataContext.StockGroupStocks.AddRange(stockGroupStocks);
                    }

                    if (groupStock.StockGroupIndicators.Count > 0)
                    {
                        var stockGroupIndicators = new List<StockGroupIndicator>();
                        foreach (var stockGroupIndicator in groupStock.StockGroupIndicators)
                        {
                            var newStockGroupIndicator = new StockGroupIndicator
                            {
                                StockGroup = newGroupStock,
                                UserId = userId,
                                Formula = stockGroupIndicator.Formula,
                                IndicatorId = stockGroupIndicator.IndicatorId,
                                CreateBy = userId,
                                CreateDate = DateTime.Now,
                                IsDeleted = 0
                            };
                            stockGroupIndicators.Add(newStockGroupIndicator);
                        }

                        _dataContext.StockGroupIndicators.AddRange(stockGroupIndicators);
                    }
                }

                await _dataContext.SaveChangesAsync();

                return ServiceResponse<StockGroupResponseDTO>.Success(null);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<StockGroupResponseDTO>> CreateStockGroupCloneByConditionGroupIdIntoOtherConditionGroupAsync(int conditionGroupIdInput, int conditionGroupIdOutput)
        {
            try
            {
                var groupStocks = await _dataContext.StockGroups
                    .Where(sg => sg.ConditionGroupId == conditionGroupIdInput && sg.IsDeleted == 0)
                    .ToListAsync();

                var conditionGroupInput = await _dataContext.ConditionGroups
                    .Where(cg => cg.Id == conditionGroupIdInput && cg.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                var userId = GetCurrentUserId();

                var conditionGroupOutput = await _dataContext.ConditionGroups
                    .Where(cg => cg.Id == conditionGroupIdOutput && cg.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                foreach (var stockGroupId in groupStocks.Select(c => c.Id))
                {
                    var groupStock = await _dataContext.StockGroups
                    .Include(sg => sg.Interval)
                    .Include(sg => sg.StockGroupStocks)
                    .Include(c => c.StockGroupIndicators)
                    .Where(sg => sg.Id == stockGroupId && sg.IsDeleted == 0)
                    .FirstOrDefaultAsync();

                    var newGroupStock = new StockGroup
                    {
                        IntervalId = groupStock.IntervalId,
                        Name = groupStock.Name,
                        ConditionGroup = conditionGroupOutput,
                        CreateBy = userId,
                        CreateDate = DateTime.Now,
                    };

                    _dataContext.StockGroups.Add(newGroupStock);

                    if (groupStock.StockGroupStocks.Count > 0)
                    {
                        var stockGroupStocks = new List<StockGroupStock>();
                        foreach (var stockGroupStock in groupStock.StockGroupStocks)
                        {
                            var newStockGroupStock = new StockGroupStock
                            {
                                StockGroup = newGroupStock,
                                StockId = stockGroupStock.StockId,
                                CreateBy = userId,
                                CreateDate = DateTime.Now,
                                IsDeleted = 0
                            };
                            stockGroupStocks.Add(newStockGroupStock);
                        }

                        _dataContext.StockGroupStocks.AddRange(stockGroupStocks);
                    }

                    if (groupStock.StockGroupIndicators.Count > 0)
                    {
                        var stockGroupIndicators = new List<StockGroupIndicator>();
                        foreach (var stockGroupIndicator in groupStock.StockGroupIndicators)
                        {
                            var newStockGroupIndicator = new StockGroupIndicator
                            {
                                StockGroup = newGroupStock,
                                UserId = userId,
                                Formula = stockGroupIndicator.Formula,
                                IndicatorId = stockGroupIndicator.IndicatorId,
                                CreateBy = userId,
                                CreateDate = DateTime.Now,
                                IsDeleted = 0
                            };
                            stockGroupIndicators.Add(newStockGroupIndicator);
                        }

                        _dataContext.StockGroupIndicators.AddRange(stockGroupIndicators);
                    }
                }

                await _dataContext.SaveChangesAsync();

                return ServiceResponse<StockGroupResponseDTO>.Success(null);
            }
            catch (Exception ex)
            {
                return ServiceResponse<StockGroupResponseDTO>.Failure(500, ex.Message);
            }
        }

        //get all stock group of user
        public async Task<ServiceResponse<List<StockGroupResponseDTO>>> GetAllStockGroupOfUser()
        {
            try
            {
                var userId = GetCurrentUserId();

                var stockGroups = await _dataContext.StockGroups
                    .Where(x => x.IsDeleted == 0 && x.CreateBy == userId).ToListAsync();

                var stockGroupResponseDTOs = _mapper.Map<List<StockGroupResponseDTO>>(stockGroups);

                return ServiceResponse<List<StockGroupResponseDTO>>.Success(stockGroupResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockGroupResponseDTO>>.Failure(500, ex.Message);
            }
        }
    }
}