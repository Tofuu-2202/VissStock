namespace VisssStock.Application.DTOs
{
    public class UpdateStockGroupIndicatorDTO
    {
        public int StockGroupId { get; set; }

        public int IndicatorId { get; set; }

        public int IntervalId { get; set; }

        public double From { get; set; }

        public double To { get; set; }
    }
}
