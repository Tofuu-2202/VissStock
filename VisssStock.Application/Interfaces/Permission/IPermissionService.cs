using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<ServiceResponse<PagedList<Permission>>> getAllPermissions(OwnerParameters ownerParameters, string? searchByName);

        Task<ServiceResponse<Permission>> getPermissionById(int id);

        Task<ServiceResponse<Permission>> updatePermission(CreatePermissionDTO createPermissionDto, int id);
        Task<ServiceResponse<Permission>> deletePermission(int id);
        Task<ServiceResponse<Permission>> createPermission(CreatePermissionDTO createPermissionDto);
    }
}
