using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Interfaces
{
    public interface IRoleService
    {
        Task<ServiceResponse<PagedList<Role>>> getAllRoles(OwnerParameters ownerParameters, string searchByName);
        Task<ServiceResponse<RoleDTOResponse>> getRoleById(int id);
        Task<ServiceResponse<RoleDTO>> updateRole(RoleDTO roleDto, int id);
        Task<ServiceResponse<Role>> deleteRole(int id);
        Task<ServiceResponse<Role>> createRole(RoleDTO roleDto);
    }
}
