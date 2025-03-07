using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class Indicator
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Formula { get; set; }

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int IsDeleted { get; set; }

    public virtual ICollection<IndicatorDraft> IndicatorDraftIndicatorId1Navigations { get; set; } = new List<IndicatorDraft>();

    public virtual ICollection<IndicatorDraft> IndicatorDraftIndicatorId2Navigations { get; set; } = new List<IndicatorDraft>();

    public virtual ICollection<StockGroupIndicator> StockGroupIndicators { get; set; } = new List<StockGroupIndicator>();
}
