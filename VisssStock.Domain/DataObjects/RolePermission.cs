using System;
using System.Collections.Generic;

namespace VisssStock.Domain.DataObjects;

public partial class RolePermission
{
    public RolePermission(int roleId, int permissionId)
    {
        this.RoleId = roleId;
        this.PermissionId = permissionId;
    }

    public int Id { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public int? UpdateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int IsDeleted { get; set; }

    public int? CreateBy { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
