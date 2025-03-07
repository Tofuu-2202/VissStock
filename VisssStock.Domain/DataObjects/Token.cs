using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class Token
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token1 { get; set; } = null!;

    public string Issue { get; set; } = null!;

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int IsDeleted { get; set; }

    public int? UserId1 { get; set; }

    public virtual User User { get; set; } = null!;
}
