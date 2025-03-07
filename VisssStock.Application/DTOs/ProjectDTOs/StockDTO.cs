using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.DTOs.ProjectDTOs
{

    public class StockRequestDTO
    {
        public string Symbol { get; set; } = null!;
        public string? Description { get; set; }
        public int TypeId { get; set; }
        public int ExchangeId { get; set; }
        public int ScreenerId { get; set; }
    }

    public class StockResponseDTO
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public string? Description { get; set; }
        public int TypeId { get; set; }
        public TypeResponseDTO? TypeResponse { get; set; }
        public int ExchangeId { get; set; }
        public ExchangeResponseDTO? ExchangeResponse { get; set; }
        public int ScreenerId { get; set; }
        public ScreenerResponseDTO? ScreenerResponse { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IsDeleted { get; set; }
    }

    public class StockListResponseDTO
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<StockResponseDTO> stockResponseDTOs { get; set; }
    }
}