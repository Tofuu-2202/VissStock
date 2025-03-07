using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VissStockApp.DTOs
{
    public class TradingViewData
    {
        public string s { get; set; }

        public List<object> d { get; set; }
    }

    public class TradingViewDatax
    {
        public string s { get; set; }

        public List<string> d { get; set; }
    }

    public class TradingViewApiResponse
    {
        public int totalCount { get; set; }
        public List<TradingViewData> data { get; set; }
    }

    public class TradingViewRequest
    {
        public SymbolsTradingView Symbols { get; set; }
        public string[] Columns { get; set; }
    }

    public class SymbolsTradingView
    {
        public string[] Tickers { get; set; }
        public QueryTradingView Query { get; set; }
    }

    public class QueryTradingView
    {
        public string[] Types { get; set; }
    }

    public class StockDataTradingView
    {
        public string Symbol { get; set; }

        public List<IndicatorTradingView> Indicator { get; set; }
    }

    public class IndicatorTradingView
    {
        public string Name { get; set; }

        public double? Value { get; set; }
    }

    public class IndicatorTradingViewx
    {
        public string Name { get; set; }

        public double PreviousValue { get; set; } = 0;

        public double CurrentValue { get; set; }
    }

    public class TradingViewRequestStock
    {
        public string Screener { get; set; }
        public List<string> Symbols { get; set; }
        public List<string> Indicator { get; set; }
    }

    public class PriorityConfig
    {
        public Dictionary<string, int> IndicatorPriority { get; set; }
        public Dictionary<string, string> ReplaceIndicator { get; set; }
        public Dictionary<string, int> support { get; set; }
        public Dictionary<string, int> resistance { get; set; }
    }

    public class TextChat
    {
        public string ChatId { get; set; }
        public string Symbol { get; set; }
        public string Interval { get; set; }
        public string Message { get; set; }
        public string? ConditionGroup { get; set; }
        public string StockGroup { get; set; }
    }

    public class AlertLog
    {   
        public string ChatId { get; set; }
        
        public List<DataJson> DataJson { get; set; }
    }

    public class AlertLogRequestDto
    {
        public string ChatId { get; set; } = null!;

        public string DataJson { get; set; } = null!;

        public string Guid { get; set; } = null!;
    }

    public class DataJson
    {
        public string Symbol { get; set; }
        public string Interval { get; set; }
        public string? ConditionGroup { get; set; }
        public string StockGroup { get; set; }
        public List<IndicatorDataJson> IndicatorDataJson { get; set; }
    }

    public class IndicatorDataJson
    {
        public string? Indicator { get; set; }
        public string? Formula { get; set; }
        public string? Value { get; set; }
    }
}
