namespace VisssStock.Application.DTOs
{
    public class IndicatorResponseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Formula { get; set; }

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int IsDeleted { get; set; }
    }
}
