using AutoMapper;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination; 
using VisssStock.Domain.DataObjects;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using VisssStock.Application.Interfaces;
using VisssStock.Infrastructure.Data;

namespace VisssStock.Application.Services.MenuServices
{
    public class MenuServiceImpl : IMenuService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public MenuServiceImpl(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<Menu>> getMenuById(int id)
        {
            var serviceResponse = new ServiceResponse<Menu>();
            var dbMenu = await _context.Menus.FirstOrDefaultAsync(c => c.Id == id);
            if (dbMenu == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Không tìm thấy menu này !";
                return serviceResponse;
            }

            serviceResponse.Data = dbMenu;
            serviceResponse.Status = true;
            return serviceResponse;
        }

        public async Task<ServiceResponse<PagedList<Menu>>> getAllMenus(OwnerParameters ownerParameters)
        {
            var serviceResponse = new ServiceResponse<PagedList<Menu>>();
            var dbMenus = await _context.Menus.Where(m => m.IsDeleted == 0).ToListAsync();
            serviceResponse.ErrorCode = 200;
            serviceResponse.Data = PagedList<Menu>.ToPagedList(
            dbMenus.AsEnumerable<Menu>().OrderBy(on => on.Id),
            ownerParameters.pageIndex,
            ownerParameters.pageSize);
            serviceResponse.Status = true;
            return serviceResponse;

        }

        public async Task<ServiceResponse<PagedList<Menu>>> getAllMenus(OwnerParameters ownerParameters, string searchByName)
        {
            var serviceResponse = new ServiceResponse<PagedList<Menu>>();
            var countqry = _context.Menus.Where(m=> m.IsDeleted == 0).CountAsync(); 

            var dbMenu = _context.Menus.OrderBy(on => on.Id).Skip(ownerParameters.pageIndex * ownerParameters.pageSize).
                    Take(ownerParameters.pageSize).ToListAsync(); 
           
            if (!String.IsNullOrEmpty(searchByName))
                dbMenu = _context.Menus.OrderBy(on => on.Id).Where(s => s.IsDeleted == 0 && s.Title.ToUpper().Contains(searchByName.ToUpper())).Skip(ownerParameters.pageIndex * ownerParameters.pageSize).
                    Take(ownerParameters.pageSize).ToListAsync();

            //wait for them both to complete
            Task.WaitAll(countqry, dbMenu);

            //use the results
            var count = countqry.Result;
            var page = dbMenu.Result;

            serviceResponse.ErrorCode = 200;
            serviceResponse.Data = new PagedList<Menu>(page, count, ownerParameters.pageIndex, ownerParameters.pageSize);
            serviceResponse.Status = true;
            return serviceResponse;
        }
        public async Task<ServiceResponse<Menu>> updateMenu(MenuBO updateMenuDto, int id)
        {
            var serviceResponse = new ServiceResponse<Menu>();
            var dbMenus = await _context.Menus.FirstOrDefaultAsync(c => c.Id == id);
            if (dbMenus == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "menu.notFoundWithId";
                return serviceResponse;
            }

            dbMenus.ParentId = updateMenuDto.ParentId;
            dbMenus.Subheader = updateMenuDto.Subheader;
            dbMenus.Title = updateMenuDto.Title;
            dbMenus.Subheader = updateMenuDto.Subheader;
            dbMenus.Path = updateMenuDto.Path;
            dbMenus.Icon = updateMenuDto.Icon;
            dbMenus.Orderno = updateMenuDto.Orderno.Value;
            _context.Menus.Update(dbMenus);
            await _context.SaveChangesAsync();
            serviceResponse.Data = dbMenus;
            serviceResponse.Status = true;
            return serviceResponse;
        }


        public async Task<ServiceResponse<Menu>> deleteMenu(int id)
        {
            var serviceResponse = new ServiceResponse<Menu>();
            var dbMenu = await _context.Menus.FirstOrDefaultAsync(c => c.Id == id);
            if (dbMenu == null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "menu.notFoundWithId";
                return serviceResponse;
            }

            dbMenu.IsDeleted = 1;
            _context.Menus.Update(dbMenu);
            await _context.SaveChangesAsync();
            serviceResponse.Data = dbMenu;
            serviceResponse.Status = true;
            return serviceResponse;
        }

        public async Task<ServiceResponse<Menu>> createMenu(MenuBO menuDto)
        {
            var serviceResponse = new ServiceResponse<Menu>();
            var menu = new Menu();
            if (_context.Menus.FirstOrDefault(x => x.Title == menuDto.Title) != null)
            {
                serviceResponse.Status = false;
                serviceResponse.ErrorCode = 400;
                serviceResponse.Message = "Menu đã tồn tại !";
                return serviceResponse;
            }

            menu.ParentId = menuDto.ParentId;
            menu.Subheader = menuDto.Subheader;
            menu.Title = menuDto.Title;
            menu.Subheader = menuDto.Subheader;
            menu.Path = menuDto.Path;
            menu.Icon = menuDto.Icon;
            menu.Orderno = 0;
            menu.IsDeleted = 0;
            var saved = _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            serviceResponse.Data = saved.Entity;
            serviceResponse.Status = true;
            return serviceResponse;
        }

    }
}
