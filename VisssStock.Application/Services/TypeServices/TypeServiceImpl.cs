using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using Type = VisssStock.Domain.DataObjects.Type;
using VisssStock.Application.Interfaces;
using AutoMapper;
namespace VisssStock.Application.Services.TypeServices

{
    public class TypeServiceImpl : ITypeService
    {
        public DataContext _dataContext;
        public IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;

        public TypeServiceImpl(DataContext dataContext, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<List<TypeResponseDTO>>> GetAllTypeAsync()
        {
            var types = await _dataContext.Types.Where(x => x.IsDeleted == 0).ToListAsync();
            var typeResponseDTOs = _mapper.Map<List<TypeResponseDTO>>(types);
            return ServiceResponse<List<TypeResponseDTO>>.Success(typeResponseDTOs);
        }

        public async Task<ServiceResponse<TypeResponseDTO>> CreateTypeAsync(TypeRequestDTO typeRequestDTO)
        {
            // Lay user id
            var userId = int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            // Tao object de save tren server
            var type = _mapper.Map<Type>(typeRequestDTO);
            type.UpdateBy = userId;
            type.CreateBy = userId;
            type.CreateDate = DateTime.Now;
            type.UpdateDate = DateTime.Now;
            type.IsDeleted = 0;

            // save va luu vao database
            await _dataContext.Types.AddAsync(type);
            await _dataContext.SaveChangesAsync();

            // chuyen ve dang response tra lai cho nguoi dung xem
            var typeResponseDTO = _mapper.Map<TypeResponseDTO>(type);
            return ServiceResponse<TypeResponseDTO>.Success(typeResponseDTO);
        }

        public async Task<ServiceResponse<TypeResponseDTO>> UpdateTypeByIdAsync(int typeId, TypeRequestDTO typeRequestDTO)
        {
            // Lay user id
            var userId = int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            // Tao object de save tren server
            var type = await _dataContext.Types.FirstOrDefaultAsync(e => e.Id == typeId);
            if (type == null)
            {
                return ServiceResponse<TypeResponseDTO>.Failure(400, "Type Not Found To Update");
            }
            // Update truong du lieu
            _mapper.Map(typeRequestDTO, type);
            type.UpdateBy = userId;
            type.UpdateDate = DateTime.Now;

            // save va luu vao database
            _dataContext.Types.Update(type);
            await _dataContext.SaveChangesAsync();

            // chuyen ve dang response tra lai cho nguoi dung xem
            var typeResponseDTO = _mapper.Map<TypeResponseDTO>(type);
            return ServiceResponse<TypeResponseDTO>.Success(typeResponseDTO);
        }

        public async Task<ServiceResponse<TypeResponseDTO>> DeleteTypeByIdAsync(int typeId)
        {
            var type = await _dataContext.Types.FirstOrDefaultAsync(e => e.Id == typeId);
            if (type == null)
            {
                return ServiceResponse<TypeResponseDTO>.Failure(400, "Type is not exist");
            }
            type.IsDeleted = 1;
            var typeResponseDto = _mapper.Map<TypeResponseDTO>(type);
            _dataContext.Types.Update(type);
            await _dataContext.SaveChangesAsync();
            return ServiceResponse<TypeResponseDTO>.Success(typeResponseDto);
        }
    }
}