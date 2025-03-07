using Microsoft.Extensions.Configuration;
using NCalc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Telegram.Bot.Types;
using VissStockApp.DTOs;
using VissStockApp;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections.Generic;
using VissStockApp.Service;

partial class Program
{
    private static Dictionary<string, TelegramBotClient> TelegramBot = new Dictionary<string, TelegramBotClient>();
    private static Dictionary<long, string> userPendingUsername = new Dictionary<long, string>();
    private static Dictionary<long, bool> userAwaitingConfirmation = new Dictionary<long, bool>();
    private static List<TextChat> textchats = new List<TextChat>();
    private static List<StockIndicator> stockIndicators = new List<StockIndicator>();

    public class ApiResponse<T>
    {
        public T Data { get; set; }
    }

    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Status { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
    }

    static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfigurationRoot configuration = builder.Build();

        var _priorityConfig = new PriorityConfig();
        configuration.GetSection("PriorityConfig").Bind(_priorityConfig);

        //bot 1 
        string botToken1 = configuration.GetSection("TOKEN:BOT_TOKEN1").Value;
        var botClient1 = new TelegramBotClient(botToken1);
        TelegramBot.Add(botToken1, botClient1);
        var cancellationTokenSource1 = new System.Threading.CancellationTokenSource();
        await TelegramBotService.StartBot(botClient1, botToken1, cancellationTokenSource1, userPendingUsername, userAwaitingConfirmation);

        //bot 2
        string botToken2 = configuration.GetSection("TOKEN:BOT_TOKEN2").Value;
        var botClient2 = new TelegramBotClient(botToken2);
        TelegramBot.Add(botToken2, botClient2);
        var cancellationTokenSource2 = new System.Threading.CancellationTokenSource();
        await TelegramBotService.StartBot(botClient2, botToken2, cancellationTokenSource2, userPendingUsername, userAwaitingConfirmation);

        _ = Task.Run(async () =>
        {

            while (true)
            {
                try
                {
                    var indicatorDraftx = await RequestApi.CallDataIndicatorDraft();

                    if (string.IsNullOrEmpty(indicatorDraftx))
                    {
                        Console.WriteLine("API response for indicator draft is null or empty.");
                        throw new Exception("API Call Data Indicator response for indicator draft is null or empty.");
                    }

                    var apiResponseDraft = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse<List<IndicatorDraftResponseDto>>>(indicatorDraftx);
                    var indicatorDrafts = new List<IndicatorDraftResponseDto>();

                    if (apiResponseDraft == null || apiResponseDraft.Data == null)
                    {
                        Console.WriteLine("API response for indicator draft is null or empty.");
                        throw new Exception("API Call Data Indicator response for indicator draft is null or empty.");
                    }
                    else
                    {
                        indicatorDrafts = apiResponseDraft.Data;
                    }

                    var response1 = await RequestApi.CallDataIndicator();

                    if (string.IsNullOrEmpty(response1))
                    {
                        Console.WriteLine("API response is null or empty.");
                        throw new Exception("API Call Data Indicator is null or empty.");
                    }

                    var apiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse<List<StockGroupResponsexDTO>>>(response1);

                    if (apiResponse == null || apiResponse.Data == null)
                    {
                        Console.WriteLine("API response is null or empty.");
                        throw new Exception("API Call Data Indicator is null or empty.");
                    }

                    Console.WriteLine("Data Tradingview Ok");
                    var stockGroupResponseDTOs = apiResponse.Data;

                    var indicatorApi = await RequestApi.GetAllIndicators();

                    if (string.IsNullOrEmpty(indicatorApi))
                    {
                        Console.WriteLine("Indicators API response is null or empty.");
                        await Task.Delay(30000);
                        continue;
                    }

                    var indicatorApiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse<List<IndicatorResponseDTO>>>(indicatorApi);

                    if (indicatorApiResponse == null || indicatorApiResponse.Data == null)
                    {
                        Console.WriteLine("Indicators API response is null or empty.");
                        await Task.Delay(30000);
                        continue;
                    }

                    var indicatorsDb = indicatorApiResponse.Data;

                    var stockDataTradingViews = new List<StockDataTradingView>();

                    //Đoạn này sau cần sửa cho nhiều sàn
                    //đây là đang giả dụ chỉ dùng cho 1 sàn viet nam
                    string screener = stockGroupResponseDTOs.FirstOrDefault()?.Stocks?.FirstOrDefault()?.ScreenerResponse?.Name ?? "vietnam";

                    List<string> symbolsx = stockGroupResponseDTOs.SelectMany(sg => sg.Stocks)
                        .Select(stock => $"{stock.ExchangeResponse.Name}:{stock.Symbol}")
                        .Distinct()
                        .ToList();

                    List<string> indicators = GetRequiredIndicators(stockGroupResponseDTOs, indicatorsDb);

                    var response2 = await RequestApi.CallTradingViewDataStockAPI(screener, symbolsx, indicators);

                    if (string.IsNullOrEmpty(response2))
                    {
                        Console.WriteLine("API response for trading data is null or empty.");
                        throw new Exception("API Call TradingView Data Stock response for trading data is null or empty.");
                    }

                    var apiResponse2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse<List<StockDataTradingView>>>(response2);

                    if (apiResponse2 == null || apiResponse2.Data == null)
                    {
                        Console.WriteLine("API response for trading data is null or empty.");
                        throw new Exception("API Call TradingView Data Stock response for trading data is null or empty.");
                    }

                    stockDataTradingViews.AddRange(apiResponse2.Data);

                    try
                    {
                        stockDataTradingViews = ComputeUserDefinedIndicators(stockDataTradingViews, stockGroupResponseDTOs, indicatorsDb);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error when Compute Indicators Stock. Detail: " + ex.Message);
                    }

                    var messagesToSend = new List<(string ChatId, string Message, string Symbol, int StockGroupId, string interval, string StockGroup, string? ConditionGroup)>();

                    var alertLogs = new List<AlertLog>();

                    foreach (var stockGroup in stockGroupResponseDTOs)
                    {
                        var alertLog = alertLogs.Where(c => c.ChatId.ToLower().Trim() == stockGroup.TelegramBotChatId.Trim()).FirstOrDefault();
                        if (alertLog == null)
                        {
                            alertLog = new AlertLog
                            {
                                ChatId = stockGroup.TelegramBotChatId,
                                DataJson = new List<DataJson>()
                            };
                        }

                        var indicatorDraftsx = indicatorDrafts.Where(c => c.StockGroupId == stockGroup.Id).ToList();

                        foreach (var stock in stockDataTradingViews)
                        {
                            var dataJson = new DataJson
                            {
                                Symbol = stock.Symbol,
                                Interval = stockGroup.Interval.Description,
                                ConditionGroup = stockGroup.ConditionGroup?.Name ?? null,
                                StockGroup = stockGroup.Name,
                                IndicatorDataJson = new List<IndicatorDataJson>()
                            };

                            var checkStock = stockGroup.Stocks.FirstOrDefault(s => s.Symbol == stock.Symbol);
                            if (checkStock == null) continue;

                            //if (stock.Symbol == "PPC" && stockGroup.ConditionGroup.Name == "Dịch vụ hạ tầng 1h")
                            //{
                            //    Console.WriteLine("stop");
                            //}

                            string message = string.Empty;

                            bool allIndicatorsInRange = true;

                            // Khởi tạo các Dictionary để lưu trữ Support và Resistance
                            Dictionary<int, string> supportValues = new Dictionary<int, string>();
                            Dictionary<int, string> resistanceValues = new Dictionary<int, string>();

                            StringBuilder messageBuilderIndicatorPriority = new StringBuilder();
                            StringBuilder messageBuilderNormalIndicator = new StringBuilder();

                            // Header with stock symbol
                            if (stockGroup.ConditionGroup != null)
                            {
                                messageBuilderIndicatorPriority.AppendLine($"Nhóm ĐK: {stockGroup.ConditionGroup.Name}");
                            }
                            messageBuilderIndicatorPriority.AppendLine($"*Mã*: *{stock.Symbol}*");
                            messageBuilderIndicatorPriority.AppendLine($"Nhóm: {stockGroup.Name}");
                            messageBuilderIndicatorPriority.AppendLine($"Thời gian: {stockGroup.Interval.Description}");
                            if (FormatNumber(stock.Indicator.Where(c => c.Name == $"volume{(string.IsNullOrEmpty(stockGroup.Interval.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}").Select(c => c.Value).FirstOrDefault()) != null)
                            {
                                messageBuilderIndicatorPriority.AppendLine($"Volume: {FormatNumber1(stock.Indicator.Where(c => c.Name == $"volume{(string.IsNullOrEmpty(stockGroup.Interval.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}").Select(c => c.Value).FirstOrDefault())}");
                            }
                            var xxxx = stock.Indicator.Distinct().ToList();

                            foreach (var indicator in stock.Indicator.Distinct().ToList())
                            {
                                var configIndicator = stockGroup.StockGroupIndicators.FirstOrDefault(x =>
                                    stockGroup.Interval.Symbol != null ?
                                    $"{x.Indicator.Name}|{stockGroup.Interval.Symbol}" == indicator.Name :
                                    x.Indicator.Name == indicator.Name);
                                if (configIndicator != null)
                                {
                                    var checkx = indicatorDraftsx.FirstOrDefault(c => c.IndicatorId2 == configIndicator.IndicatorId || c.IndicatorId1 == configIndicator.IndicatorId);

                                    if (checkx != null)
                                    {
                                        var existingStockIndicator = stockIndicators.FirstOrDefault(si => si.Name == stock.Symbol);

                                        if (existingStockIndicator != null)
                                        {
                                            // Update the PreviousValue with the current value before adding the new value
                                            var existingIndicator = existingStockIndicator.indicators.FirstOrDefault(ind => ind.Name == indicator.Name);

                                            if (existingIndicator != null)
                                            {
                                                if (existingIndicator.CurrentValue != indicator.Value)
                                                {
                                                    existingIndicator.PreviousValue = existingIndicator.CurrentValue;
                                                    existingIndicator.CurrentValue = (double)indicator.Value;
                                                }
                                            }
                                            else
                                            {
                                                // Add a new indicator to the existing StockIndicator
                                                existingStockIndicator.indicators.Add(new IndicatorTradingViewx
                                                {
                                                    Name = indicator.Name,
                                                    PreviousValue = 0,  // No previous value since it's new
                                                    CurrentValue = (double)indicator.Value
                                                });
                                            }
                                        }
                                        else
                                        {
                                            // Add a new StockIndicator with the current indicator
                                            stockIndicators.Add(new StockIndicator
                                            {
                                                Name = stock.Symbol,
                                                indicators = new List<IndicatorTradingViewx>
                                                {
                                                    new IndicatorTradingViewx
                                                    {
                                                        Name = indicator.Name,
                                                        PreviousValue = 0,  // Initial value is null as there's no previous value
                                                        CurrentValue = (double) indicator.Value
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                            }

                            foreach (var indicator in stock.Indicator.Distinct().ToList())
                            {
                                var configIndicator = stockGroup.StockGroupIndicators.FirstOrDefault(x =>
                                    stockGroup.Interval.Symbol != null ?
                                    $"{x.Indicator.Name}|{stockGroup.Interval.Symbol}" == indicator.Name :
                                    x.Indicator.Name == indicator.Name);

                                var parts = indicator.Name.Split("|");
                                var xxx = parts[0];
                                var yyy = parts.Length > 1 ? parts[1] : null;

                                if (_priorityConfig.ReplaceIndicator.ContainsKey(xxx) && yyy == stockGroup.Interval.Symbol)
                                {
                                    var indicatorName = _priorityConfig.ReplaceIndicator[xxx];

                                    // Check if the indicator is a support or resistance level
                                    if (_priorityConfig.support.ContainsKey(xxx))
                                    {
                                        int index = _priorityConfig.support[xxx];
                                        supportValues[index] = FormatNumber1(indicator.Value);
                                    }
                                    else if (_priorityConfig.resistance.ContainsKey(xxx))
                                    {
                                        int index = _priorityConfig.resistance[xxx];
                                        resistanceValues[index] = FormatNumber1(indicator.Value);
                                    }
                                    else
                                    {
                                        messageBuilderIndicatorPriority.AppendLine($"{indicatorName}: {FormatNumber1(indicator.Value)}");

                                        IndicatorDataJson indicatorDataJson = new IndicatorDataJson
                                        {
                                            Indicator = indicatorName,
                                            Formula = FormatNumber1(indicator.Value)
                                        };

                                        dataJson.IndicatorDataJson.Add(indicatorDataJson);
                                    }
                                }

                                if (configIndicator != null)
                                {
                                    var expression = configIndicator.Formula;
                                    var checkExpression = expression.Replace("stock", indicator.Value.ToString());

                                    if (indicator.Value == null || DataProcess.CheckFormula(checkExpression))
                                    {
                                        if (!_priorityConfig.ReplaceIndicator.ContainsKey(configIndicator.Indicator.Name))
                                        {
                                            messageBuilderNormalIndicator.AppendLine($"- {configIndicator.Indicator.Name}: {FormatNumber(indicator.Value)}");

                                            IndicatorDataJson indicatorDataJson = new IndicatorDataJson
                                            {
                                                Indicator = configIndicator.Indicator.Name,
                                                Formula = FormatNumber(indicator.Value)
                                            };

                                            dataJson.IndicatorDataJson.Add(indicatorDataJson);
                                        }
                                    }
                                    else
                                    {
                                        allIndicatorsInRange = false;
                                        break;
                                    }
                                }
                            }

                            if (allIndicatorsInRange)
                            {
                                Console.WriteLine(stock.Symbol + " ok");

                                // Add Support values to the message
                                if (supportValues.Any())
                                {
                                    var sortedSupports = supportValues.OrderBy(kv => kv.Key).Select(kv => kv.Value);
                                    messageBuilderIndicatorPriority.AppendLine($"S: {string.Join("/", sortedSupports)}");
                                    IndicatorDataJson indicatorDataJson = new IndicatorDataJson
                                    {
                                        Indicator = "Support",
                                        Formula = string.Join("/", sortedSupports)
                                    };
                                    dataJson.IndicatorDataJson.Add(indicatorDataJson);
                                }

                                // Add Resistance values to the message
                                if (resistanceValues.Any())
                                {
                                    var sortedResistances = resistanceValues.OrderBy(kv => kv.Key).Select(kv => kv.Value);
                                    messageBuilderIndicatorPriority.AppendLine($"R: {string.Join("/", sortedResistances)}");
                                    IndicatorDataJson indicatorDataJson = new IndicatorDataJson
                                    {
                                        Indicator = "Resistance",
                                        Formula = string.Join("/", sortedResistances)
                                    };
                                }

                                // Combine the two parts of the message
                                StringBuilder finalMessageBuilder = new StringBuilder();
                                finalMessageBuilder.Append(messageBuilderIndicatorPriority.ToString());

                                if (messageBuilderNormalIndicator.Length > 0)
                                {
                                    finalMessageBuilder.AppendLine("Chi tiết:");
                                    finalMessageBuilder.AppendLine(messageBuilderNormalIndicator.ToString());
                                }

                                foreach (var indicatorDraft in indicatorDraftsx)
                                {
                                    var indicatorName1 = $"{indicatorDraft.IndicatorId1Navigation.Name}{(string.IsNullOrEmpty(stockGroup.Interval.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}";

                                    var indicatorName2 = $"{indicatorDraft.IndicatorId2Navigation.Name}{(string.IsNullOrEmpty(stockGroup.Interval.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}";

                                    var stockIndicator = stockIndicators.FirstOrDefault(i => i.Name == stock.Symbol);

                                    if (stockIndicator == null) continue;

                                    var indicator1 = stockIndicator.indicators
                                    .Where(i => i.Name == $"{indicatorDraft.IndicatorId1Navigation.Name}{(string.IsNullOrEmpty(stockGroup.Interval.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}")
                                    .FirstOrDefault();

                                    if (indicator1 == null || indicator1.PreviousValue == 0) continue;

                                    var indicator2 = stockIndicator.indicators
                                    .Where(i => i.Name == $"{indicatorDraft.IndicatorId2Navigation.Name}{(string.IsNullOrEmpty(stockGroup.Interval.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}")
                                    .FirstOrDefault();

                                    if (indicator2 == null || indicator2.PreviousValue == 0) continue;

                                    if (indicatorDraft.Type == "Crossing")
                                    {
                                        bool isCrossing = IsCrossingCal(indicator1.PreviousValue, indicator1.CurrentValue, indicator2.PreviousValue, indicator2.CurrentValue);

                                        if (isCrossing)
                                        {
                                            finalMessageBuilder.AppendLine($"*{indicatorDraft.Type}*: {indicatorDraft.IndicatorId1Navigation.Name} và {indicatorDraft.IndicatorId2Navigation.Name}");
                                        }
                                    }
                                }

                                var chatIdx = stockGroup.TelegramBotChatId;
                                if (!string.IsNullOrEmpty(chatIdx))
                                {
                                    try
                                    {
                                        messagesToSend
                                        .Add((ChatId: chatIdx,
                                        Message: finalMessageBuilder.ToString(),
                                        Symbol: stock.Symbol,
                                        StockGroupId: stockGroup.Id,
                                        interval: stockGroup.Interval.Description,
                                        StockGroup: stockGroup.Name,
                                        ConditionGroup: stockGroup.ConditionGroup?.Name ?? null));
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }

                                alertLog.DataJson.Add(dataJson);
                            }
                        }
                        var check = alertLogs.Where(c => c.ChatId.ToLower().Trim() == alertLog.ChatId.ToLower().Trim()).FirstOrDefault();
                        if(check == null)
                        {
                            alertLogs.Add(alertLog);
                        }
                    }

                    TimeZoneInfo hanoiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                    DateTime hanoiTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hanoiTimeZone);

                    var sortedMessages = messagesToSend.OrderBy(m => m.ChatId).ThenBy(c => c.Symbol).ThenBy(m => m.StockGroupId).ToList();

                    // Lặp qua từng ChatId duy nhất
                    var distinctChatIds = sortedMessages.Select(m => m.ChatId).Distinct();

                    foreach (var chatId in distinctChatIds)
                    {
                        var alertLog = alertLogs.FirstOrDefault(c => c.ChatId == chatId.ToString());
                        if (alertLog != null)
                        {
                            Console.WriteLine($"ChatId: {chatId}");
                        }

                            // Kiểm tra xem có tin nhắn nào cần gửi cho ChatId này không
                        bool hasMessagesToSendForChatId = sortedMessages.Any(message =>
                        {
                            return message.ChatId == chatId && !textchats.Any(c =>
                                c.ChatId == message.ChatId &&
                                c.Symbol == message.Symbol &&
                                c.Interval == message.interval &&
                                (message.ConditionGroup == null || c.ConditionGroup == message.ConditionGroup)
                            );
                        });

                        if (hasMessagesToSendForChatId)
                        {
                            // Gửi timestamp cho ChatId nếu có tin nhắn cần gửi
                            var timestampMessage = $"-----------{hanoiTime:dd/MM/yyyy HH:mm:ss}-----------";
                            await TelegramBotService.SendMessageToChatAsync(TelegramBot, chatId, timestampMessage);

                            // Gửi các tin nhắn tương ứng cho ChatId này
                            foreach (var message in sortedMessages.Where(m => m.ChatId == chatId))
                            {
                                var checkChat = textchats.FirstOrDefault(c => c.ChatId == message.ChatId &&
                                                                        c.Symbol == message.Symbol &&
                                                                        c.Interval == message.interval &&
                                                                        (message.ConditionGroup == null || c.ConditionGroup == message.ConditionGroup));

                                if (checkChat != null)
                                {
                                    if (checkChat.Message != message.Message)
                                    {
                                        await TelegramBotService.SendMessageToChatAsync(TelegramBot, message.ChatId, message.Message);
                                        textchats.Remove(checkChat);
                                        textchats.Add(new TextChat
                                        {
                                            ChatId = message.ChatId,
                                            Message = message.Message,
                                            Symbol = message.Symbol,
                                            Interval = message.interval,
                                            ConditionGroup = message.ConditionGroup ?? null,
                                            StockGroup = message.StockGroup
                                        });
                                    }
                                    else
                                    {
                                        if (alertLog != null)
                                        {
                                            alertLog.DataJson.RemoveAll(c => c.Symbol == message.Symbol && c.Interval == message.interval && c.ConditionGroup == message.ConditionGroup && c.StockGroup == message.StockGroup);
                                        }
                                    }
                                }
                                else
                                {
                                    await TelegramBotService.SendMessageToChatAsync(TelegramBot, message.ChatId, message.Message);
                                    textchats.Add(new TextChat
                                    {
                                        ChatId = message.ChatId,
                                        Message = message.Message,
                                        Symbol = message.Symbol,
                                        Interval = message.interval,
                                        ConditionGroup = message.ConditionGroup ?? null,
                                        StockGroup = message.StockGroup
                                    });
                                }
                            }
                            //CreateAlertLog
                            if (alertLog != null && alertLog.DataJson.Count > 0)
                            {
                                var alertLogRequestDto = new AlertLogRequestDto
                                {
                                    ChatId = alertLog.ChatId,
                                    DataJson = JsonConvert.SerializeObject(alertLog.DataJson),
                                    Guid = Guid.NewGuid().ToString()
                                };

                                var response = await RequestApi.CreateAlertLog(alertLogRequestDto);
                            }
                        }


                    }

                    Console.WriteLine($"-----------{hanoiTime:dd/MM/yyyy HH:mm:ss}-----------");
                    await Task.Delay(30000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    await TelegramBotService.SendMessageToChatAsync(TelegramBot, "VissStock1;7254594402:AAETdkjBi6syET3Zcej3qymlP0XN-VLqTDI;6542597563", ex.Message);
                    await Task.Delay(30000);
                }
            }
            
        });
        Console.ReadLine();
    }

    private static string FormatNumber(double? value)
    {
        if (value == null)
        {
            return null;
        }
        // Chuyển đổi giá trị từ double sang decimal để làm tròn chính xác
        decimal decimalValue = (decimal)value;

        // Làm tròn giá trị với hai chữ số thập phân
        decimal roundedValue = Math.Round(decimalValue, 2);

        // Chuyển đổi giá trị thành chuỗi với định dạng phân cách hàng nghìn
        string formattedValue = roundedValue.ToString("N2");

        // Nếu giá trị sau dấu thập phân là 00, loại bỏ phần thập phân
        if (formattedValue.EndsWith(".00"))
        {
            formattedValue = roundedValue.ToString("N0");
        }

        return formattedValue;
    }

    private static string FormatNumber1(double? value)
    {
        if (value == null)
        {
            return null;
        }
        // Chuyển đổi giá trị từ double sang decimal để làm tròn chính xác
        decimal decimalValue = (decimal)value;

        // Làm tròn giá trị với hai chữ số thập phân
        decimal roundedValue = Math.Round(decimalValue, 0);

        // Chuyển đổi giá trị thành chuỗi với định dạng phân cách hàng nghìn
        string formattedValue = roundedValue.ToString("N0");

        return formattedValue;
    }

    public static List<StockDataTradingView> ComputeUserDefinedIndicators(
    List<StockDataTradingView> stockDataTradingViews,
    List<StockGroupResponsexDTO> stockGroupResponseDTOs,
    List<IndicatorResponseDTO> indicatorsDb)
    {
        foreach (var stock in stockDataTradingViews)
        {
            var stockGroups = stockGroupResponseDTOs
                .Where(sg => sg.Stocks.Any(s => s.Symbol == stock.Symbol))
                .ToList();

            foreach(var stockGroup in stockGroups)
            {
                foreach (var configIndicator in stockGroup.StockGroupIndicators)
                {
                    if (string.IsNullOrEmpty(configIndicator.Indicator.Formula)) continue;

                    // Check if the indicator with the specific interval has already been computed
                    var existingIndicator = stock.Indicator
                        .FirstOrDefault(i => i.Name == $"{configIndicator.Indicator.Name}{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}");
                    if (existingIndicator != null) continue;

                    // Compute the indicator including its dependencies
                    ComputeIndicator(stock, configIndicator, indicatorsDb, stockGroup.Interval.Symbol);
                }

                var defaulIndicator = new List<string>
                {
                    "volume", "close", "pivot_point", "support1", "support2", "support3", "resistance1", "resistance2", "resistance3"
                };

                var indicatorx = indicatorsDb.Where(c => defaulIndicator.Contains(c.Name)).ToList();

                foreach(var xxx in indicatorx)
                {
                    ComputeIndicator1(stock, xxx, indicatorsDb, stockGroup.Interval.Symbol);
                }
            }
        }
        return stockDataTradingViews;
    }

    private static void ComputeIndicator(
            StockDataTradingView stock,
            StockGroupIndicatorxResponseDTO configIndicator,
            List<IndicatorResponseDTO> indicatorsDb,
            string? intervalSymbol)
    {
        if (string.IsNullOrEmpty(configIndicator.Indicator.Formula)) return;

        var formula = configIndicator.Indicator.Formula;
        formula = formula.Substring(1);
        var regex = new Regex(@"@(\w+(\.\w+)*)");
        var placeholders = new HashSet<string>();

        foreach (Match match in regex.Matches(formula))
        {
            placeholders.Add(match.Value.Substring(1)); // Remove '@'
        }

        // Resolve dependencies recursively
        var placeholderValues = new Dictionary<string, string>();
        foreach (var placeholder in placeholders)
        {
            var indicatorKey = $"{placeholder}{(string.IsNullOrEmpty(intervalSymbol) ? string.Empty : $"|{intervalSymbol}")}";
            var indicator = stock.Indicator.FirstOrDefault(i => i.Name == indicatorKey);
            if (indicator != null)
            {
                placeholderValues[placeholder] = indicator.Value.ToString();
            }
            else
            {
                // Compute the value for the placeholder recursively
                var placeholderConfig = indicatorsDb
                    .FirstOrDefault(i => i.Name == placeholder);

                if (placeholderConfig != null && !string.IsNullOrEmpty(placeholderConfig.Formula))
                {
                    // Create a temporary configIndicator for the placeholder
                    var placeholderConfigIndicator = new StockGroupIndicatorxResponseDTO
                    {
                        Indicator = placeholderConfig,
                    };

                    // Recursively compute the placeholder indicator
                    ComputeIndicator(stock, placeholderConfigIndicator, indicatorsDb, intervalSymbol);
                }

                // Recheck if the placeholder value is now available
                indicator = stock.Indicator.FirstOrDefault(i => i.Name == indicatorKey);
                if (indicator != null)
                {
                    placeholderValues[placeholder] = indicator.Value.ToString();
                }
            }
        }

        var formulaWithValues = formula;
        foreach (var placeholder in placeholderValues)
        {
            formulaWithValues = formulaWithValues.Replace($"@{placeholder.Key}", placeholder.Value);
        }

        // Evaluate the formula (assuming you have a method to do so)
        var result = EvaluateFormula(formulaWithValues);

        var indicatorAdd = stock.Indicator.FirstOrDefault(i => i.Name == $"{configIndicator.Indicator.Name}{(string.IsNullOrEmpty(intervalSymbol) ? string.Empty : $"|{intervalSymbol}")}");
        if (indicatorAdd == null)
        {
            // Add the computed result to the stock data
            stock.Indicator.Add(new IndicatorTradingView
            {
                Name = $"{configIndicator.Indicator.Name}{(string.IsNullOrEmpty(intervalSymbol) ? string.Empty : $"|{intervalSymbol}")}",
                Value = result,
            });
        }
    }

    private static void ComputeIndicator1(
        StockDataTradingView stock,
        IndicatorResponseDTO indicatorx,
        List<IndicatorResponseDTO> indicatorsDb,
        string? intervalSymbol)
    {
        if (string.IsNullOrEmpty(indicatorx.Formula)) return;

        var formula = indicatorx.Formula;
        formula = formula.Substring(1);
        var regex = new Regex(@"@(\w+(\.\w+)*)");
        var placeholders = new HashSet<string>();

        foreach (Match match in regex.Matches(formula))
        {
            placeholders.Add(match.Value.Substring(1)); // Remove '@'
        }

        // Resolve dependencies recursively
        var placeholderValues = new Dictionary<string, string>();
        foreach (var placeholder in placeholders)
        {
            var indicatorKey = $"{placeholder}{(string.IsNullOrEmpty(intervalSymbol) ? string.Empty : $"|{intervalSymbol}")}";
            var indicator = stock.Indicator.FirstOrDefault(i => i.Name == indicatorKey);
            if (indicator != null)
            {
                placeholderValues[placeholder] = indicator.Value.ToString();
            }
            else
            {
                // Compute the value for the placeholder recursively
                var placeholderConfig = indicatorsDb
                    .FirstOrDefault(i => i.Name == placeholder);

                if (placeholderConfig != null && !string.IsNullOrEmpty(placeholderConfig.Formula))
                {
                    //// Create a temporary configIndicator for the placeholder
                    //var placeholderConfigIndicator = new StockGroupIndicatorxResponseDTO
                    //{
                    //    Indicator = placeholderConfig,
                    //};

                    // Recursively compute the placeholder indicator
                    ComputeIndicator1(stock, placeholderConfig, indicatorsDb, intervalSymbol);
                }

                // Recheck if the placeholder value is now available
                indicator = stock.Indicator.FirstOrDefault(i => i.Name == indicatorKey);
                if (indicator != null)
                {
                    placeholderValues[placeholder] = indicator.Value.ToString();
                }
            }
        }

        var formulaWithValues = formula;
        foreach (var placeholder in placeholderValues)
        {
            formulaWithValues = formulaWithValues.Replace($"@{placeholder.Key}", placeholder.Value);
        }

        // Evaluate the formula (assuming you have a method to do so)
        var result = EvaluateFormula(formulaWithValues);

        //check
        var indicatorAdd = stock.Indicator.FirstOrDefault(i => i.Name == $"{indicatorx.Name}{(string.IsNullOrEmpty(intervalSymbol) ? string.Empty : $"|{intervalSymbol}")}");

        // Add the computed result to the stock data
        if (indicatorAdd == null)
        {
            stock.Indicator.Add(new IndicatorTradingView
            {
                Name = $"{indicatorx.Name}{(string.IsNullOrEmpty(intervalSymbol) ? string.Empty : $"|{intervalSymbol}")}",
                Value = result,
            });
        }
    }

    private static double EvaluateFormula(string formula)
    {
        try
        {
            // Xử lý các phép toán không được hỗ trợ trực tiếp (như Abs) trước khi truyền vào NCalc
            formula = ProcessSpecialFunctions(formula);

            // Tạo biểu thức NCalc
            var expression = new Expression(formula);

            // Đánh giá công thức
            var result = expression.Evaluate();

            // Trả về kết quả dưới dạng double
            return Convert.ToDouble(result);
        }
        catch (Exception ex)
        {
            // Xử lý lỗi đánh giá công thức
            Console.WriteLine($"Error evaluating formula: {formula}. Exception: {ex.Message}");
            return 0.0; // Trả về giá trị mặc định hoặc xử lý lỗi theo cách bạn muốn
        }
    }

    private static string ProcessSpecialFunctions(string formula)
    {
        //if (formula.Contains("Abs"))
        //{
        //    Console.WriteLine($"Processing Abs function in formula: {formula}");
        //}
        // Xử lý hàm Abs
        var regexAbs = new Regex(@"Abs\(([^)]+)\)");
        formula = regexAbs.Replace(formula, match =>
        {
            // Lấy biểu thức bên trong hàm Abs
            var innerExpression = match.Groups[1].Value;

            // Đánh giá biểu thức bên trong hàm Abs
            var expression = new Expression(innerExpression);
            var result = expression.Evaluate();

            // Tính giá trị tuyệt đối
            return Math.Abs(Convert.ToDouble(result)).ToString();
        });

        // Trả về công thức đã xử lý
        return formula;
    }

    private static List<string> GetRequiredIndicators(
       List<StockGroupResponsexDTO> stockGroupResponseDTOs,
       List<IndicatorResponseDTO> indicatorsDb)
    {
        var requiredIndicators = new HashSet<string>();

        foreach (var stockGroup in stockGroupResponseDTOs ?? Enumerable.Empty<StockGroupResponsexDTO>())
        {
            // Process base indicators (those with null formulas)
            var baseIndicators = stockGroup.StockGroupIndicators
                .Where(indicator => !string.IsNullOrEmpty(indicator.Indicator.Formula))
                .Select(indicator => $"{indicator.Indicator.Name}|{stockGroup.Interval?.Symbol ?? string.Empty}")
                .Where(indicator => !string.IsNullOrEmpty(indicator))
                .Distinct()
                .ToList();

            baseIndicators.AddRange(new List<string>
            {
                $"volume{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}",
                $"close{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}",
                $"pivot_point{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}",
                $"support1{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}",
                $"support2{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}",
                $"support3{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}",
                $"resistance1{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}",
                $"resistance2{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}",
                $"resistance3{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}"
            });


            baseIndicators = baseIndicators.Distinct().ToList();

            foreach (var indicator in baseIndicators)
            {
                var parts = indicator.Split('|');
                if (parts.Length == 2)
                {
                    var name = parts[0];
                    var interval = parts[1];
                    ResolveDependencies(name, interval, requiredIndicators, indicatorsDb);
                }
                else if (parts.Length == 1)
                {
                    ResolveDependencies(parts[0], string.Empty, requiredIndicators, indicatorsDb);
                }
            }
            var baseIndicatorsx = stockGroup.StockGroupIndicators
                .Where(indicator => string.IsNullOrEmpty(indicator.Indicator.Formula))
                .Select(indicator => $"{indicator.Indicator.Name}{(string.IsNullOrEmpty(stockGroup.Interval?.Symbol) ? string.Empty : $"|{stockGroup.Interval.Symbol}")}")
                .Distinct()
                .ToList();

            foreach (var baseIndicatorx in baseIndicatorsx)
            {
                requiredIndicators.Add(baseIndicatorx);
            }
        }

        return requiredIndicators.Distinct().ToList();
    }


    private static void ResolveDependencies(
        string indicatorName,
        string interval,
        HashSet<string> requiredIndicators,
        List<IndicatorResponseDTO> indicatorsDb)
    {
        // Check if this indicator is already included
        var indicatorKey = $"{indicatorName}{(string.IsNullOrEmpty(interval) ? string.Empty : $"|{interval}")}";
        if (requiredIndicators.Contains(indicatorKey)) return;



        var dbIndicator = indicatorsDb.FirstOrDefault(dbInd => dbInd.Name == indicatorName);
        if (string.IsNullOrEmpty(dbIndicator.Formula))
        {
            requiredIndicators.Add($"{indicatorName}{(string.IsNullOrEmpty(interval) ? string.Empty : $"|{interval}")}");
        };

        var formula = dbIndicator.Formula;
        if (formula == null) return; // Check for null formula

        var matches = Regex.Matches(formula, @"@(\w+(\.\w+)*)");

        foreach (Match match in matches)
        {
            var placeholder = match.Value.Substring(1);
            var dependentIndicator = indicatorsDb.FirstOrDefault(ind => ind.Name == placeholder);

            if (!string.IsNullOrEmpty(dependentIndicator.Formula))
            {
                // Recursively resolve dependencies for the found base indicators
                ResolveDependencies(placeholder, interval, requiredIndicators, indicatorsDb);
            }
            else
            {
                requiredIndicators.Add($"{placeholder}{(string.IsNullOrEmpty(interval) ? string.Empty : $"|{interval}")}");
            }
        }
    }


    public static double EvaluateIndicator(string formula, Dictionary<string, double> variables)
    {
        // Tạo một đối tượng Expression từ thư viện ncalc
        Expression expression = new Expression(formula);

        // Gán giá trị cho các biến trong công thức
        foreach (var variable in variables)
        {
            expression.Parameters[variable.Key] = variable.Value;
        }

        // Tính toán và trả về kết quả
        return (double)expression.Evaluate();
    }


    //public static Dictionary<string, double> CalculatePivot(double close, double high, double low)
    //{
    //    double pivotPoint = Math.Round((high + low + close) / 3, 3);

    //    double support1 = Math.Round((2 * pivotPoint) - high, 3);
    //    double support2 = Math.Round(pivotPoint - (high - low), 3);
    //    double support3 = Math.Round(low - 2 * (high - pivotPoint), 3);

    //    double resistance1 = Math.Round((2 * pivotPoint) - low, 3);
    //    double resistance2 = Math.Round(pivotPoint + (high - low), 3);
    //    double resistance3 = Math.Round(high + 2 * (pivotPoint - low), 3);

    //    return new Dictionary<string, double>
    //    {
    //        { "pivot_point", pivotPoint },
    //        { "support1", support1 },
    //        { "support2", support2 },
    //        { "support3", support3 },
    //        { "resistance1", resistance1 },
    //        { "resistance2", resistance2 },
    //        { "resistance3", resistance3 }
    //    };
    //}

    public static bool IsCrossingCal(double previousIndicator1, double currentIndicator1, double previousIndicator2, double currentIndicator2)
    {
        var calc = (currentIndicator1 - previousIndicator1) * (currentIndicator2 - previousIndicator2);

        if (calc < 0)
        {
            return true;
        }
        else
        {
           return false;
        }
    }
}
