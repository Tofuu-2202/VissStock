using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class Interval
{
    public int Id { get; set; }

    public string? Symbol { get; set; }

    public string Description { get; set; } = null!;

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int IsDeleted { get; set; }

    public virtual ICollection<StockGroup> StockGroups { get; set; } = new List<StockGroup>();
}
