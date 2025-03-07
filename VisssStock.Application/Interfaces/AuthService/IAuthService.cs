using VisssStock.Application.DTOs;
using VisssStock.Application.Models; 
using VisssStock.Domain.DataObjects;
using Microsoft.AspNetCore.Mvc;

namespace VisssStock.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<UserDTO>> createSingleUser(CreateUserDTO createUserDto);
        Task<LoginResponse> login(LoginFrom loginFrom);

        Task<ServiceResponse<UploadExcelFileResponse>> createMultiUser(UploadExcelFileRequest uploadExcelFileRequest);

        Task<ServiceResponse<ResetPasswordForm>> resetPassword(ResetPasswordForm resetPasswordForm,int userId);
        Task<ServiceResponse<IActionResult>> resetPasswordAdmin(int id);

        
        Task<ServiceResponse<UpdateProfileDTO>> updateProfile(UpdateProfileDTO updateProfile,int userId);

        Task<ServiceResponse<List<MenuDTO>>>getMenuItems(int userId);
        Task<ServiceResponse<List<MenuDTO>>> getMenuItemsV2(int userId);

    }
}
