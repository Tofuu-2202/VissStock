namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class ScreenerRequestDTO
    {
        public string Name { get; set; } = null!;
    }

    public class ScreenerResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IsDeleted { get; set; }
    }
}