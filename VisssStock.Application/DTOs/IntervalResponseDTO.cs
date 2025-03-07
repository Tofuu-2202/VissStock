namespace VisssStock.Application.DTOs
{
    public class IntervalResponseDTO
    {
        public int Id { get; set; }

        public string Symbol { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int IsDeleted { get; set; }
    }
}
