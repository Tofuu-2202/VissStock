using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class StockGroupIndicator
{
    public int Id { get; set; }

    public int StockGroupId { get; set; }

    public int IndicatorId { get; set; }

    public int UserId { get; set; }

    public string? Formula { get; set; }

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int IsDeleted { get; set; }

    public int? IsActive { get; set; }

    public virtual Indicator Indicator { get; set; } = null!;

    public virtual StockGroup StockGroup { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
