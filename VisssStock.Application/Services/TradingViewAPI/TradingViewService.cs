using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Text.Json;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using VisssStock.Application.Interfaces;
using ZstdSharp;
using Type = VisssStock.Domain.DataObjects.Type;

namespace VisssStock.Application.Services.TradingViewAPI
{
    public class TradingViewService : ITradingViewService
    {
        private readonly HttpClient _httpClient;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TradingViewService(DataContext context, IMapper mapper, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _context = context;
            _mapper = mapper;
            _httpClient = httpClient;
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

        public async Task<ServiceResponse<TradingViewApiResponse>> GetTradingViewData(string screener)
        {
            var stocks = await _context.Stocks
                .Include(c => c.Exchange)
                .Include(c => c.Screener)
                .Where(c => c.IsDeleted == 0).ToListAsync();

            var existingExchanges = await _context.Exchanges.ToListAsync();

            var existingTypes = await _context.Types.ToListAsync();

            var existingScreeners = await _context.Screeners.ToListAsync();

            var request = new TradingViewRequest
            {
                Symbols = new SymbolsTradingView
                {
                    Tickers = new string[] { },
                    Query = new QueryTradingView
                    {
                        Types = new string[] {"stock", "index"}
                    }
                },
                Columns = new string[] { "description", "type", "exchange" }
            };

            var apiUrl = $"https://scanner.tradingview.com/{screener}/scan";
            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStreamAsync();
                var tradingViewResponse = await JsonSerializer.DeserializeAsync<TradingViewApiResponse>(responseData);

                if (tradingViewResponse != null && tradingViewResponse.data != null)
                {
                    var data = tradingViewResponse.data;

                    foreach (var item in data)
                    {
                        var exchangeSymbol = item.s.ToString();
                        var exchange = item.d[2].ToString();
                        var symbol = exchangeSymbol.Split(":")[1].ToString();
                        var description = item.d[0].ToString();
                        var type = item.d[1].ToString();

                        // Insert or update exchange
                        var existingExchange = existingExchanges
                            .FirstOrDefault(e => e.Name == exchange);
                        if (existingExchange == null)
                        {
                            existingExchange = new Exchange
                            {
                                Name = exchange,
                                CreateBy = 20,
                                UpdateBy = 20,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                IsDeleted = 0
                            };
                            await _context.Exchanges.AddAsync(existingExchange);
                            await _context.SaveChangesAsync();

                            existingExchanges.Add(existingExchange);
                        }

                        // Insert or update exchange
                        var existingType = existingTypes
                            .FirstOrDefault(e => e.Name == type);
                        if (existingType == null)
                        {
                            existingType = new Type
                            {
                                Name = type,
                                CreateBy = 20,
                                UpdateBy = 20,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                IsDeleted = 0
                            };
                            await _context.Types.AddAsync(existingType);
                            await _context.SaveChangesAsync();

                            existingTypes.Add(existingType);
                        }

                        // Insert or update screener
                        var existingScreener = existingScreeners
                            .FirstOrDefault(s => s.Name == screener);
                        if (existingScreener == null)
                        {
                            existingScreener = new Screener
                            {
                                Name = screener,
                                CreateBy = 20,
                                UpdateBy = 20,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                IsDeleted = 0
                            };
                            await _context.Screeners.AddAsync(existingScreener);
                            await _context.SaveChangesAsync();

                            existingScreeners.Add(existingScreener);
                        }

                        // Insert or update stock
                        var existingStock = stocks
                            .FirstOrDefault(s => s.Symbol == symbol && s.Exchange.Name == exchange);
                        if (existingStock != null)
                        {
                            existingStock.Description = description;
                            existingStock.Type = existingType;
                            existingStock.Exchange = existingExchange;
                            existingStock.Screener = existingScreener;
                            _context.Stocks.Update(existingStock);
                        }
                        else
                        {
                            var newStock = new Stock
                            {
                                Symbol = symbol,
                                Description = description,
                                Type = existingType,
                                Exchange = existingExchange,
                                Screener = existingScreener,
                                CreateBy = 20,
                                UpdateBy = 20,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                IsDeleted = 0
                            };
                            await _context.Stocks.AddAsync(newStock);
                        }
                    }

                    await _context.SaveChangesAsync();

                    return ServiceResponse<TradingViewApiResponse>.Success(tradingViewResponse);
                }
                else
                {
                    return new ServiceResponse<TradingViewApiResponse>
                    {
                        Status = false,
                        ErrorCode = 400,
                        Message = "Không tìm thấy dữ liệu"
                    };
                }
            }

            return new ServiceResponse<TradingViewApiResponse>();
        }

        public async Task<ServiceResponse<List<IndicatorResponseDTO>>> getAllIndicators()
        {
            try
            {
                // Truy vấn dữ liệu từ bảng Indicators
                var dbIndicator = _context.Indicators
                    .Where(c => c.IsDeleted == 0);
                // Áp dụng phân trang với PagedList
                var indicators = await dbIndicator.ToListAsync();
                var indicatorResponseDTOs = _mapper.Map<List<IndicatorResponseDTO>>(indicators);

                return ServiceResponse<List<IndicatorResponseDTO>>.Success(indicatorResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<IndicatorResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<List<StockDataTradingView>>> GetTradingViewDataStock(TradingViewRequestStock requestStock)
        {
            var request = new TradingViewRequest
            {
                Symbols = new SymbolsTradingView
                {
                    Tickers = requestStock.Symbols.ToArray(),
                },
                Columns = requestStock.Indicator.ToArray()
            };

            var request_json = JsonSerializer.Serialize(request);

            var apiUrl = $"https://scanner.tradingview.com/{requestStock.Screener}/scan";
            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStreamAsync();
                var tradingViewResponse = await JsonSerializer.DeserializeAsync<TradingViewApiResponse>(responseData);

                if (tradingViewResponse != null && tradingViewResponse.data != null)
                {
                    var datas = tradingViewResponse.data;
                    var stockDataList = new List<StockDataTradingView>();

                    foreach (var item in datas)
                    {
                        var exchangeSymbol = item.s;
                        var symbol = exchangeSymbol.Split(":")[1];

                        var indicators = new List<IndicatorTradingView>();
                        for (int i = 0; i < item.d.Count; i++)
                        {
                            var indicatorValue = ConvertObjectToDouble(item.d[i]); // Use the conversion method
                            indicators.Add(new IndicatorTradingView
                            {
                                Name = requestStock.Indicator[i],
                                Value = indicatorValue
                            });
                        }

                        stockDataList.Add(new StockDataTradingView
                        {
                            Symbol = symbol,
                            Indicator = indicators
                        });
                    }

                    return ServiceResponse<List<StockDataTradingView>>.Success(stockDataList);
                }
                else
                {
                    return ServiceResponse<List<StockDataTradingView>>.Failure(404, "Không tìm thấy dữ liệu");
                }
            }

            return ServiceResponse<List<StockDataTradingView>>.Failure((int)response.StatusCode, "Lỗi khi gọi API TradingView");
        }

        private double? ConvertObjectToDouble(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (obj is JsonElement element && element.ValueKind == JsonValueKind.Number)
            {
                return element.GetDouble();
            }
            else if (double.TryParse(obj.ToString(), out var value))
            {
                return value;
            }
            else
            {
                throw new InvalidCastException("Cannot convert object to double.");
            }
        }


        public async Task<ServiceResponse<List<StockGroupResponsexDTO>>> GetAllStockGroupIndicators()
        {
            try
            {
                var query = from sg in _context.StockGroups
                            join cg in _context.ConditionGroups on sg.ConditionGroupId equals cg.Id
                            join sgs in _context.StockGroupStocks on sg.Id equals sgs.StockGroupId
                            join s in _context.Stocks on sgs.StockId equals s.Id
                            join sc in _context.Screeners on s.ScreenerId equals sc.Id
                            join t in _context.Types on s.TypeId equals t.Id
                            join ex in _context.Exchanges on s.ExchangeId equals ex.Id
                            join tb in _context.TelegramBots on sg.TelegramBotId equals tb.Id into telegramBots
                            from tb in telegramBots.DefaultIfEmpty() // Sử dụng tb ở đây
                            join utb in _context.UserTelegrams on tb.Id equals utb.TelegramBotId into userTelegrams 
                            from utb in userTelegrams.DefaultIfEmpty()
                            join sgi in _context.StockGroupIndicators on sg.Id equals sgi.StockGroupId into stockGroupIndicators
                            from sgi in stockGroupIndicators.DefaultIfEmpty() // Đảm bảo biến này không bị khai báo lại
                            join i in _context.Indicators on sgi.IndicatorId equals i.Id into indicators
                            from i in indicators.DefaultIfEmpty()
                            join iv in _context.Intervals on sg.IntervalId equals iv.Id into intervals
                            from iv in intervals.DefaultIfEmpty()
                            join u in _context.Users on sgi.UserId equals u.Id into users
                            from u in users.DefaultIfEmpty()
                            where sg.IsDeleted == 0
                                  && s.IsDeleted == 0
                                  && utb.UserId == sg.CreateBy
                                  && sg.IsActive > 0
                                  && (sgi == null || sgi.IsActive > 0) // Chỉ kiểm tra sgi nếu nó không null
                                  && (i == null || i.IsDeleted == 0)
                                  && (iv == null || iv.IsDeleted == 0)
                                  && (u == null || u.IsDeleted == 0)
                                  && utb != null
                            orderby sgs.IsLike descending
                            select new
                            {
                                TelegramChatId = utb.ChatId,
                                TelegramBot = tb,
                                ConditionGroup = cg,
                                StockGroup = sg,
                                StockGroupIndicator = sgi,
                                Stock = s,
                                Indicator = i,
                                Interval = iv,
                                User = u,
                                Type = t,
                                Exchange = ex,
                                Screen = sc
                            };

                var result = await query.ToListAsync();

                var stockGroupResponseDTOs = result.GroupBy(item => item.StockGroup)
                    .Select(group =>
                    {
                        var stockGroupDto = _mapper.Map<StockGroupResponsexDTO>(group.Key);

                        // Loại bỏ trùng lặp cho StockGroupIndicators
                        stockGroupDto.StockGroupIndicators = group
                            .Where(item => item.StockGroupIndicator != null)
                            .Select(item =>
                            {
                                var stockGroupIndicatorDto = _mapper.Map<StockGroupIndicatorxResponseDTO>(item.StockGroupIndicator);
                                stockGroupIndicatorDto.Indicator = _mapper.Map<IndicatorResponseDTO>(item.Indicator);
                                stockGroupIndicatorDto.User = _mapper.Map<UserDTO>(item.User);
                                return stockGroupIndicatorDto;
                            })
                            .DistinctBy(indicator => new { indicator.Id}) // Distinct bằng cách so sánh các thuộc tính quan trọng
                            .ToList();

                        // Loại bỏ trùng lặp cho Stocks
                        stockGroupDto.Stocks = group
                            .Select(item =>
                            {
                                var stockDto = _mapper.Map<StockResponseDTO>(item.Stock);
                                stockDto.ScreenerResponse = _mapper.Map<ScreenerResponseDTO>(item.Screen);
                                stockDto.TypeResponse = _mapper.Map<TypeResponseDTO>(item.Type);
                                stockDto.ExchangeResponse = _mapper.Map<ExchangeResponseDTO>(item.Exchange);
                                return stockDto;
                            })
                            .DistinctBy(stock => new { stock.Id, stock.Symbol }) // Distinct bằng cách so sánh các thuộc tính quan trọng
                            .ToList();

                        stockGroupDto.Interval = _mapper.Map<IntervalResponseDTO>(group.Select(c => c.Interval).FirstOrDefault());

                        stockGroupDto.ConditionGroup = _mapper.Map<ConditionGroupResponsexDto>(group.Select(c => c.ConditionGroup).FirstOrDefault());

                        if(!string.IsNullOrEmpty(group.Select(c => c.TelegramBot).FirstOrDefault().TelegramBotName) && !string.IsNullOrEmpty(group.Select(c => c.TelegramBot).FirstOrDefault().Token) && !string.IsNullOrEmpty(group.Select(c => c.TelegramBot).FirstOrDefault().Token))
                        {
                            stockGroupDto.TelegramBotChatId = $"{group.Select(c => c.TelegramBot).FirstOrDefault().TelegramBotName};{group.Select(c => c.TelegramBot).FirstOrDefault().Token};{group.Select(c => c.TelegramChatId).FirstOrDefault()}";
                        }

                        return stockGroupDto;
                    }).ToList();

                return ServiceResponse<List<StockGroupResponsexDTO>>.Success(stockGroupResponseDTOs);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockGroupResponsexDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<List<StockGroupResponsexDTO>>> UpdateUserTelegram(TelegramSaveRequestDTO requestDTO)
        {
            try
            {

                //get user Id
                var telegramBot = _context.TelegramBots.Where(c => c.Token == requestDTO.Token).FirstOrDefault();

                if(telegramBot != null)
                {
                    var user = await _context.Users.Where(c => c.UserName.ToLower() == requestDTO.UserName.ToLower()).FirstOrDefaultAsync();
                    var userTeleEx = _context.UserTelegrams.Where(c => c.UserId == user.Id && c.TelegramBotId == telegramBot.Id).FirstOrDefault();
                    if(userTeleEx == null)
                    {
                        var newUserTelegramBot = new UserTelegram
                        {
                            UserId = user.Id,
                            ChatId = requestDTO.TelegramChatId,
                            TelegramBotId = telegramBot.Id
                        };
                        _context.UserTelegrams.Add(newUserTelegramBot);
                    }
                    else
                    {
                        userTeleEx.ChatId = requestDTO.TelegramChatId;

                        _context.UserTelegrams.Update(userTeleEx);
                    }
                    await _context.SaveChangesAsync();
                }
                return ServiceResponse<List<StockGroupResponsexDTO>>.Success(null);
            }
            catch (Exception ex)
            {
                return ServiceResponse<List<StockGroupResponsexDTO>>.Failure(500, ex.Message);
            }
        }
    }
}
