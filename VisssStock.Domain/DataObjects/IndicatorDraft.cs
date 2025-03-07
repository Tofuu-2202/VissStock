using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class IndicatorDraft
{
    public int Id { get; set; }

    public int StockGroupId { get; set; }

    public int IndicatorId1 { get; set; }

    public int IndicatorId2 { get; set; }

    public string Type { get; set; } = null!;

    public virtual Indicator IndicatorId1Navigation { get; set; } = null!;

    public virtual Indicator IndicatorId2Navigation { get; set; } = null!;

    public virtual StockGroup StockGroup { get; set; } = null!;
}
