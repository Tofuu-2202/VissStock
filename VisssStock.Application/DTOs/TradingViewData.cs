namespace VisssStock.Application.DTOs
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

    public class  IndicatorTradingView
    {
        public string Name { get; set; }

        public double? Value { get; set; }
    }

    //string screener, List<string> symbols, List<string> Indicator
    public class TradingViewRequestStock
    {
        public string Screener { get; set; }
        public List<string> Symbols { get; set; }
        public List<string> Indicator { get; set; }
    }
}
