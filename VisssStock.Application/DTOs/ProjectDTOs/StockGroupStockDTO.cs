namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class StockGroupStockRequestDTO
    {
        public int StockId { get; set; }
        public int StockGroupId { get; set; }
    }

    public class StockGroupStockResponseDTO
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public StockResponseDTO Stock { get; set; }
        public int StockGroupId { get; set; }
        public StockGroupResponseDTO StockGroup { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IsDeleted { get; set; }
    }
}