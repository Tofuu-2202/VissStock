using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class Transaction
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TypeId { get; set; }

    public int StockId { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }

    public int Time { get; set; }

    public virtual Stock Stock { get; set; } = null!;

    public virtual TransactionType Type { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
