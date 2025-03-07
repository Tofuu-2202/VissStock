using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class Stock
{
    public int Id { get; set; }

    public string Symbol { get; set; } = null!;

    public int TypeId { get; set; }

    public int ExchangeId { get; set; }

    public int ScreenerId { get; set; }

    public string? Description { get; set; }

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int IsDeleted { get; set; }

    public virtual Exchange Exchange { get; set; } = null!;

    public virtual Screener Screener { get; set; } = null!;

    public virtual ICollection<StockGroupStock> StockGroupStocks { get; set; } = new List<StockGroupStock>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual Type Type { get; set; } = null!;
}
