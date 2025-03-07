namespace VisssStock.Application.DTOs
{
    public class AlertLogRequestDto
    {
        public string ChatId { get; set; } = null!;

        public string DataJson { get; set; } = null!;

        public string Guid { get; set; } = null!;
    }

    public class AlertLogResponseDto
    {
        public int Id { get; set; }

        public string ChatId { get; set; } = null!;

        public string DataJson { get; set; } = null!;

        public AlertLogDataConverted Data { get; set; }

        public int CreateAt { get; set; }

        public string Guid { get; set; } = null!;
    }

    public class AlertLogDataConverted
    {
        public long ChatId { get; set; }

        public List<DataJson> DataJson { get; set; }
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
        public string Indicator { get; set; }
        public string Formula { get; set; }
        public string? Value { get; set; }
    }

    public class AlertLogFillterDto
    {
        public string? Indicator { get; set; }
        public string? ConditionGroup { get; set; }
        public string? Interval { get; set; }
        public string? Symbol { get; set; }
        public string? StockGroup { get; set; }
        public int? FromTime { get; set; }
        public string? Guid { get; set; }
    }
}
