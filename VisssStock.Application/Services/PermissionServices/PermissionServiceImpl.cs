using AutoMapper;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Domain.DataObjects;
using Microsoft.EntityFrameworkCore;
using VisssStock.Infrastructure.Data;
using VisssStock.Application.Interfaces;
namespace VisssStock.Application.Services.PermissionServices
{
    public class PermissionServiceImpl : IPermissionService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PermissionServiceImpl(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PagedList<Permission>>> getAllPermissions(OwnerParameters ownerParameters,
        string? searchByName)
        {
            var serviceResponse = new ServiceResponse<PagedList<Permission>>();
            var dbPermissions = await _context.Permissions.ToListAsync();
            if (searchByName != null)
            {
                dbPermissions = dbPermissions.Where(x => x.Name.ToUpper().Contains(searchByName.ToUpper())).ToList();
            }

            serviceResponse.Data = PagedList<Permission>.ToPagedList(
            dbPermissions.AsEnumerable<Permission>().OrderBy(on => on.Name),
            ownerParameters.pageIndex,
            ownerParameters.pageSize);
            serviceResponse.Status = true;
            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> countAllPermissions()
        {
            var serviceResponse = new ServiceResponse<int>();
            var count = _context.Permissions.Count();
            serviceResponse.ErrorCode = 200;
            serviceResponse.Data = count;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Permission>> getPermissionById(int id)
        {
            var serviceResponse = new ServiceResponse<Permission>();
            var dbPermissions = await _context.Permissions.FirstOrDefaultAsync(c => c.Id == id);
            if (dbPermissions == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "permission.notFoundWithId";
                return serviceResponse;
            }

            serviceResponse.Data = dbPermissions;
            serviceResponse.Status = true;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Permission>> updatePermission(CreatePermissionDTO createPermissionDto, int id)
        {
            var serviceResponse = new ServiceResponse<Permission>();

            var dbPermisisonExist =
            _context.Permissions.FirstOrDefault(x => x.Name == createPermissionDto.Name && x.Id != id);
            if (dbPermisisonExist != null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Đã tồn tại !";
                return serviceResponse;
            }


            var dbPermisison = await _context.Permissions.FirstOrDefaultAsync(c => c.Id == id);
            if (dbPermisison == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy !";
                return serviceResponse;
            }


            dbPermisison.Name = createPermissionDto.Name;
            dbPermisison.Description = createPermissionDto.Description;
            _context.Permissions.Update(dbPermisison);
            await _context.SaveChangesAsync();
            serviceResponse.Data = dbPermisison;
            serviceResponse.Status = true;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Permission>> deletePermission(int id)
        {
            var serviceResponse = new ServiceResponse<Permission>();
            var dbPermission = await _context.Permissions.FirstOrDefaultAsync(c => c.Id == id);
            if (dbPermission == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "permission.notFoundWithId";
                return serviceResponse;
            }

            dbPermission.IsDeleted = 1;
            var deleteList = _context.RolePermissions.Where(rp => rp.PermissionId == id).ToList();
            foreach (var rolePermission in deleteList)
            {
                rolePermission.IsDeleted = 1;
            }

            _context.Permissions.Update(dbPermission);
            await _context.SaveChangesAsync();
            serviceResponse.Status = true;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Permission>> createPermission(CreatePermissionDTO createPermissionDto)
        {
            var serviceResponse = new ServiceResponse<Permission>();
            var permission = new Permission();
            permission.Name = createPermissionDto.Name;
            permission.Description = createPermissionDto.Description;
            if (validateCommon(createPermissionDto) == false)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "permission.nameAlreadyExist";
                return serviceResponse;
            }

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            serviceResponse.Data = permission;
            serviceResponse.Status = true;
            return serviceResponse;
        }

        private bool validateCommon(CreatePermissionDTO createPermissionDto)
        {
            var dbPermission = _context.Permissions.FirstOrDefault(c => c.Name == createPermissionDto.Name);
            if (dbPermission != null)
            {
                return false;
            }

            return true;
        }
    }
}
