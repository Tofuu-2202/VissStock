using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class StockGroupIndicatorRequestDTO
    {
        public int StockGroupId { get; set; }
        public int IndicatorId { get; set; }
        public int IntervalId { get; set; }
        public string Formula { get; set; }
        public int IsActive { get; set; }
    }
    public class StockGroupIndicatorResponseDTO2
    {
        public int Id { get; set; }

        public int StockGroupId { get; set; }

        public int IndicatorId { get; set; }

        public int IntervalId { get; set; }
        public string? Formula { get; set; }
        public int UserId { get; set; }

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int IsDeleted { get; set; }

        public int IsActive { get; set; }

        public virtual IndicatorResponseDTO Indicator { get; set; } = null!;

        public virtual IntervalResponseDTO Interval { get; set; } = null!;

        public virtual StockGroupResponseDTO StockGroup { get; set; } = null!;

        public virtual UserDTO User { get; set; } = null!;
    }
}
