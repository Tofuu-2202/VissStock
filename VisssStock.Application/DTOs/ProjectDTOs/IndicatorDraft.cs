using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class IndicatorDraftResponseDto
    {
        public int Id { get; set; }

        public int StockGroupId { get; set; }

        public int IndicatorId1 { get; set; }

        public int IndicatorId2 { get; set; }

        public string Type { get; set; } = null!;

        public IndicatorResponseDTO IndicatorId1Navigation { get; set; }

        public IndicatorResponseDTO IndicatorId2Navigation { get; set; }

        public StockGroupResponse1DTO StockGroup { get; set; }
    }

    public class IndicatorDraftRequestDto
    {
        public int StockGroupId { get; set; }
        public int IndicatorId1 { get; set; }
        public int IndicatorId2 { get; set; }
        public string Type { get; set; } = null!;
    }
}
