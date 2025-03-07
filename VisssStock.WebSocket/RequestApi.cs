using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VissStockApp.DTOs;

namespace VissStockApp
{
    internal class RequestApi
    {
        private static readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://160.30.113.15:8061/"),
            Timeout = TimeSpan.FromMinutes(5)
        };


        public class TelegramSaveRequestDTO
        {
            public string Token { get; set; }
            public string UserName { get; set; }
            public string TelegramChatId { get; set; }
            public string TelegramUsername { get; set; }
        }

        public static async Task<string> CallTradingViewDataStockAPI(string screener, List<string> symbols, List<string> indicators)
        {
            try
            {
                var requestData = new TradingViewRequestStock
                {
                    Screener = screener,
                    Symbols = symbols,
                    Indicator = indicators.Distinct().ToList()
                };

                var requestData_json = JsonConvert.SerializeObject(requestData);

                var content = new StringContent(requestData_json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/TradingView/GetTradingViewDataStock", content);
                response.EnsureSuccessStatusCode(); // Ném ngoại lệ nếu HTTP status code không phải 2xx

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when Calling TradingView Data Stock. Detail: " + ex.Message);
            }
            
        }

        public static async Task<string> CreateAlertLog(AlertLogRequestDto requestDto)
        {
            var requestData_json = JsonConvert.SerializeObject(requestDto);

            var content = new StringContent(requestData_json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/AlertLog/CreateAlertLog", content);
            response.EnsureSuccessStatusCode(); // Ném ngoại lệ nếu HTTP status code không phải 2xx

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> CallUpdateUserAPI(string token, string userName, string chatId, string userameTelegram)
        {
            try
            {
                var requestData = new TelegramSaveRequestDTO
                {
                    Token = token,
                    UserName = userName,
                    TelegramChatId = chatId,
                    TelegramUsername = userameTelegram
                };

                var requestData_json = JsonConvert.SerializeObject(requestData);

                var content = new StringContent(requestData_json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/TradingView/UpdateUserTelegram", content);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when Calling Update User API. Detail: " + ex.Message);
            }

        }

        //data cấu hình
        public static async Task<string> CallDataIndicator()
        {
            try
            {
                var requestData = new { };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/TradingView/GetAllStockGroupIndicators", content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when Calling Data Indicator. Detail: " + ex.Message);
            }
        }

        public static async Task<string> CallDataIndicatorDraft()
        {
            try
            {
                var requestData = new { };

                var jsonContent = JsonConvert.SerializeObject(requestData);

                var response = await client.GetAsync("/api/IndicatorDraft/getAllIndicatorDrafts");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex) 
            {
                throw new Exception("Error when Calling Data Indicator Draft. Detail: " + ex.Message);
            }
        }

        public static async Task<string> GetAllIndicators()
        {
            try
            {
                var requestData = new { };

                var jsonContent = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/TradingView/GetAllIndicators", content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when Calling Get All Indicators. Detail: " + ex.Message);
            }
        }
    }
}
