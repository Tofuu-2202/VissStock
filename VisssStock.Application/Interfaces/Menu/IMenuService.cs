using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination; 
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Interfaces
{
    public interface IMenuService
    {
        Task<ServiceResponse<Menu>> getMenuById(int id);
        Task<ServiceResponse<PagedList<Menu>>> getAllMenus(OwnerParameters ownerParameters);
        Task<ServiceResponse<PagedList<Menu>>> getAllMenus(OwnerParameters ownerParameters, string searchByName);
        Task<ServiceResponse<Menu>> updateMenu(MenuBO menuDto, int id);
        Task<ServiceResponse<Menu>> deleteMenu(int id);
        Task<ServiceResponse<Menu>> createMenu(MenuBO menuDto);
    }
}
