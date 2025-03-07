using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class TelegramBot
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public string TelegramBotName { get; set; } = null!;

    public virtual ICollection<StockGroup> StockGroups { get; set; } = new List<StockGroup>();
}
