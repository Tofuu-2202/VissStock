using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class TypeRequestDTO
    {
        public string Name { get; set; } = null!;
    }
    public class TypeResponseDTO
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
