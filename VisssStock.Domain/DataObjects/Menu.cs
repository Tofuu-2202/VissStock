using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class Menu
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public string? Subheader { get; set; }

    public string? Title { get; set; }

    public string? Path { get; set; }

    public string? Icon { get; set; }

    public int? CreateBy { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? IsDeleted { get; set; }

    public int? Orderno { get; set; }

    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
