using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class Exchange
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int IsDeleted { get; set; }

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
