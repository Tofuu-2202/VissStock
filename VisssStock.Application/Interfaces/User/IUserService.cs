using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDTO>> getRoles(int userid);

        Task<ServiceResponse<PagedList<UserDTO>>> getAllUsers(OwnerParameters ownerParameters, string? searchByName,
        int? roleId, string? searchByEmail, int? userId);

        Task<ServiceResponse<PagedList<UserDTO>>> getAllUserWithInfor(OwnerParameters ownerParameters,
        string? searchByName, int? roleId, string? searchByEmail, int? userId);

        Task<ServiceResponse<UserDTO>> getUserById(int id);

        Task<ServiceResponse<UpdateUserDTO>> updateUser(UpdateUserDTO updateUserDto, int id);
        Task<ServiceResponse<UserDTO>> editActiveUser(int id);

        Task<ServiceResponse<User>> createUsers(User user, int roleId, string subjectId);

    }
}
