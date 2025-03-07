using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.DTOs.ProjectDTOs
{
    public class StockGroupRequestDTO
    {
        public string Name { get; set; } = null!;

        public int IntervalId { get; set; }

        public int? ConditionGroupId { get; set; }
        public int TelegramBotId { get; set; } = 1;

    }

    public class StockGroupFilterDto
    {
        public string? Keyword { get; set; } = null!;
        public int? IntervalId { get; set; }

        public int? ConditionGroupId { get; set; }

        public int? TelegramBotId { get; set; }
    }

    public class StockGroupResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ConditionGroupId { get; set; }
        public ConditionGroupResponsexDto ConditionGroup { get; set; } = null!;
        public int IntervalId { get; set; }
        public IntervalResponseDTO Interval { get; set; } = null!;
        public int TelegramBotId { get; set; }
        public TelegramBotResponsexDTO TelegramBot { get; set; } = null!;
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IsDeleted { get; set; }
        public int IsActive { get; set; }
    }

    public class TelegramBotResponsexDTO
    {
        public int Id { get; set; }
        public string TelegramBotName { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string ChatId { get; set; } = null!;
    }

    public class StockGroupResponse1DTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public ConditionGroupResponseDto ConditionGroup { get; set; } = null!;

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int IsDeleted { get; set; }

        public int? IsActive { get; set; }

        public int IntervalId { get; set; }
    }

    public class StockGroupResponsexDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ConditionGroupResponsexDto ConditionGroup { get; set; } = null!;
        public int IntervalId { get; set; }
        public IntervalResponseDTO Interval { get; set; } = null!;
        public string TelegramBotChatId { get; set; } = null!; // name:token:chatId
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IsDeleted { get; set; }
        public int IsActive { get; set; }
        public List<StockGroupIndicatorxResponseDTO> StockGroupIndicators { get; set; }
        public List<StockResponseDTO> Stocks { get; set; }
    }
}