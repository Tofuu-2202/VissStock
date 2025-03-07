using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class RoleMenu
{
    public int Id { get; set; }

    public int? RoleId { get; set; }

    public int? MenuId { get; set; }

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? IsDeleted { get; set; }

    public virtual Menu? Menu { get; set; }

    public virtual Role? Role { get; set; }
}
