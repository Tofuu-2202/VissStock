namespace VisssStock.Application.DTOs
{
    public class StockGroupStocksResponseDTO
    {
        public int Id { get; set; }

        public int StockId { get; set; }

        public int StockGroupId { get; set; }

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int IsDeleted { get; set; }

    }
}
