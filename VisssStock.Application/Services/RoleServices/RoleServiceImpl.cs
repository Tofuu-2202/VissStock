using System.Net;
using AutoMapper;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Domain.DataObjects;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using VisssStock.Application.Interfaces;
using VisssStock.Infrastructure.Data;

namespace VisssStock.Application.Services.RoleServices
{
    public class RoleServiceImpl : IRoleService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RoleServiceImpl(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public RoleServiceImpl()
        {
        }

        public async Task<ServiceResponse<PagedList<Role>>> getAllRoles(OwnerParameters ownerParameters,
        string searchByName)
        {
            var serviceResponse = new ServiceResponse<PagedList<Role>>();
            var dbRoles = await _context.Roles.Where(x => x.Name.ToLower()
            .Contains(string.IsNullOrEmpty(searchByName) ? "" : searchByName.ToLower()))
            .ToListAsync();
            serviceResponse.Data = PagedList<Role>.ToPagedList(
            dbRoles.AsEnumerable<Role>().OrderByDescending(on => on.Id),
            ownerParameters.pageIndex,
            ownerParameters.pageSize);
            return serviceResponse;
        }

        public async Task<ServiceResponse<RoleDTOResponse>> getRoleById(int id)
        {
            var serviceResponse = new ServiceResponse<RoleDTOResponse>();

            var dbRoles = await _context.Roles.FirstOrDefaultAsync(c => c.Id == id);
            if (dbRoles == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy vai trò này !";
                return serviceResponse;
            }
            else
            {
                var _roleDTO = _mapper.Map<RoleDTOResponse>(dbRoles);
                _roleDTO.Permission = (from role in _context.Roles
                                       join rolePermission in _context.RolePermissions on role.Id equals rolePermission.RoleId
                                       join permission in _context.Permissions on rolePermission.PermissionId equals permission.Id
                                       where role.Id == id
                                       select permission).Distinct().ToListAsync().Result;
                _roleDTO.Menu = (from role in _context.Roles
                                 join roleMenu in _context.RoleMenus on role.Id equals roleMenu.RoleId
                                 join menu in _context.Menus on roleMenu.MenuId equals menu.Id
                                 where role.Id == id
                                 select menu).Distinct().ToListAsync().Result;
                serviceResponse.Data = _roleDTO;
                serviceResponse.Status = true;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<RoleDTO>> updateRole(RoleDTO updateRoleDto, int id)
        {
            var serviceResponse = new ServiceResponse<RoleDTO>();
            var dbRoles = _context.Roles.FirstOrDefault(c => c.Id == id);
            if (dbRoles == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy vai trò này !";
                return serviceResponse;
            }
            else
            {
                dbRoles.Name = updateRoleDto.Name;
                dbRoles.Description = updateRoleDto.Description;
                foreach (var permissionId in updateRoleDto.PermissionId)
                {
                    var rolePermission = new RolePermission(dbRoles.Id, permissionId);
                    _context.RolePermissions.Add(rolePermission);
                }

                foreach (var menuId in updateRoleDto.MenuId)
                {
                    var roleMenu = new RoleMenu();
                    roleMenu.RoleId = dbRoles.Id;
                    roleMenu.MenuId = menuId;
                    _context.RoleMenus.Add(roleMenu);
                }

                _mapper.Map(updateRoleDto, dbRoles);
                serviceResponse.Data = _mapper.Map<RoleDTO>(dbRoles);
                (from rp in _context.RolePermissions
                 where rp.RoleId == id
                 select rp).ToList()
                .ForEach(x => x.IsDeleted = 1);

                (from rm in _context.RoleMenus
                 where rm.RoleId == id
                 select rm).ToList()
                .ForEach(x => x.IsDeleted = 1);
                await _context.SaveChangesAsync();
                serviceResponse.Status = true;
            }
            return serviceResponse;
        }


        public async Task<ServiceResponse<Role>> deleteRole(int id)
        {
            var serviceResponse = new ServiceResponse<Role>();
            var dbRole = await _context.Roles.FirstOrDefaultAsync(c => c.Id == id);
            if (dbRole == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy vai trò này !";
                return serviceResponse;
            }
            else
            {
                dbRole.IsDeleted = 1;
                _context.Roles.Update(dbRole);
                await _context.SaveChangesAsync();
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Role>> createRole(RoleDTO roleDto)
        {
            var serviceResponse = new ServiceResponse<Role>();
            if (await _context.Roles.FirstOrDefaultAsync(x => x.Name == roleDto.Name) != null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Vai trò đã tồn tại !";
                return serviceResponse;
            }
            else
            {
                var role = new Role();
                role.Name = roleDto.Name;
                role.Description = roleDto.Description;
                var saved = _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                foreach (var permissionId in roleDto.PermissionId)
                {
                    var rolePermission = new RolePermission(saved.Entity.Id, permissionId);
                    _context.RolePermissions.Add(rolePermission);
                }

                foreach (var menuId in roleDto.MenuId)
                {
                    var roleMenu = new RoleMenu();
                    roleMenu.RoleId = saved.Entity.Id;
                    roleMenu.MenuId = menuId;
                    _context.RoleMenus.Add(roleMenu);
                }

                await _context.SaveChangesAsync();
                serviceResponse.Data = saved.Entity;
            }

            return serviceResponse;
        }

    }
}
