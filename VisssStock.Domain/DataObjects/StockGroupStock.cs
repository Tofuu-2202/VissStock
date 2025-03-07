using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class StockGroupStock
{
    public int Id { get; set; }

    public int StockId { get; set; }

    public int StockGroupId { get; set; }

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int IsDeleted { get; set; }

    public int IsLike { get; set; }

    public virtual Stock Stock { get; set; } = null!;

    public virtual StockGroup StockGroup { get; set; } = null!;
}
