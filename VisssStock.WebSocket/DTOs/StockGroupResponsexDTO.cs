using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VissStockApp.DTOs
{
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

    public class ConditionGroupResponsexDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class StockResponseDTO
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = null!;
        public string? Description { get; set; }
        public int TypeId { get; set; }
        public TypeResponseDTO TypeResponse { get; set; }
        public int ExchangeId { get; set; }
        public ExchangeResponseDTO ExchangeResponse { get; set; }
        public int ScreenerId { get; set; }
        public ScreenerResponseDTO ScreenerResponse { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IsDeleted { get; set; }
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

    public class ExchangeResponseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int IsDeleted { get; set; }
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

    public class StockGroupIndicatorxResponseDTO
    {
        public int Id { get; set; }

        public int StockGroupId { get; set; }

        public int IndicatorId { get; set; }

        public IndicatorResponseDTO Indicator { get; set; }

        public string Formula { get; set; }

        public int UserId { get; set; }

        public UserDTO User { get; set; }

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int IsDeleted { get; set; }
    }

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

    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int IsTeacher { get; set; }
        public int Enable { get; set; }
        public string? TelegramChatId { get; set; }
        public string? TelegramUsername { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int IsDeleted { get; set; }
        public List<RoleDTOUser> roles { get; set; }
    }

    public class RoleDTOUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

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

    public class StockGroupResponse1DTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int? CreateBy { get; set; }

        public int? UpdateBy { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public int IsDeleted { get; set; }

        public int? IsActive { get; set; }

        public int IntervalId { get; set; }
    }


    public class StockIndicator
    {
        public string Name { get; set; } = null!;

        public List<IndicatorTradingViewx> indicators { get; set; }
    }

}
