using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;


public partial class AlertLog
{
    public int Id { get; set; }

    public string ChatId { get; set; } = null!;

    public string DataJson { get; set; } = null!;

    public int CreateAt { get; set; }

    public string Guid { get; set; } = null!;
}
