using VisssStock.Application.DTOs.ProjectDTOs;

namespace VisssStock.Application.DTOs
{
    public class StockGroupIndicatorResponseDTO
    {
        public int Id { get; set; }

        public int StockGroupId { get; set; }

        public StockGroupResponseDTO StockGroup { get; set; }

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

        public int IsActive { get; set; }
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
        public int IsActive { get; set; }
    }

    public class StockTradingViewDto
    {
        public int Id { get; set; }

        public string StockName { get; set; }

        public string Exchange { get; set; }

        public string Sceener { get; set; }

        public string StockName_Exchange { get; set; }

    }

    public class UserTradingViewDto
    {
        public int Id { get; set; }

        public string ChatId { get; set; }

        public string TelegramUsername { get; set; }
    }
}
