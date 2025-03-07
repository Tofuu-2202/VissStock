using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class StockGroup
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

    public int? ConditionGroupId { get; set; }

    public int TelegramBotId { get; set; }

    public virtual ConditionGroup? ConditionGroup { get; set; }

    public virtual ICollection<IndicatorDraft> IndicatorDrafts { get; set; } = new List<IndicatorDraft>();

    public virtual Interval Interval { get; set; } = null!;

    public virtual ICollection<StockGroupIndicator> StockGroupIndicators { get; set; } = new List<StockGroupIndicator>();

    public virtual ICollection<StockGroupStock> StockGroupStocks { get; set; } = new List<StockGroupStock>();

    public virtual TelegramBot TelegramBot { get; set; } = null!;
}
