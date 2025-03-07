using System;
using System.Collections.Generic; 

namespace VisssStock.Domain.DataObjects
{
    public partial class RoleDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<int> PermissionId { get; set; }
        
        public List<int> MenuId { get; set; }
    }

    public class RoleDTOResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<Permission> Permission { get; set; }
        public List<Menu> Menu { get; set; }
    }
}
