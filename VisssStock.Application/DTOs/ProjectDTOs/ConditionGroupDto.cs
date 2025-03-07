namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class ConditionGroupRequestDto
    {
        public string Name { get; set; } = null!;
    }

    public class ConditionGroupResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

    
        public List<StockGroupResponse1DTO> Groups { get; set; } = null!;
    }

    public class ConditionGroupFillterDto
    {
        public string Keyword { get; set; } = null!;
    }

    public class ConditionGroupResponsexDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    //pagedlist
    public class ConditionGroupPagedListResponseDto
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<ConditionGroupResponsexDto> ConditionGroups { get; set; }
    }
}
