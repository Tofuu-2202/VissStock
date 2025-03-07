using AutoMapper;
//using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;
using VisssStock.Application.Interfaces;

namespace VisssStock.Application.Services.ExchangeServices
{
    public class ExchangeServiceImpl : IExchangeService
    {
        public DataContext _dataContext;
        public IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;

        public ExchangeServiceImpl(DataContext dataContext, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<PagedListResponseDTO<ExchangeResponseDTO>>> GetAllExchangeAsync(OwnerParameters ownerParameters)
        {
            try
            {
                var exchanges = await _dataContext.Exchanges.Where(x => x.IsDeleted == 0).ToListAsync();
                var exchangeResponseDTOs = _mapper.Map<List<ExchangeResponseDTO>>(exchanges);

                // Áp dụng phân trang với PagedList
                var exchangesPagedList = PagedList<ExchangeResponseDTO>.ToPagedList(exchangeResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);

                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<ExchangeResponseDTO>(exchangesPagedList);
                return ServiceResponse<PagedListResponseDTO<ExchangeResponseDTO>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<ExchangeResponseDTO>>.Failure(500, ex.Message);
            }
        }
        public async Task<ServiceResponse<ExchangeResponseDTO>> CreateExchangeAsync(ExchangeRequestDTO exchangeRequestDTO)
        {
            // Lay user id
            var userId = int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            // Tao object de save tren server
            var exchange = _mapper.Map<Exchange>(exchangeRequestDTO);
            exchange.UpdateBy = userId;
            exchange.CreateBy = userId;
            exchange.CreateDate = DateTime.Now;
            exchange.UpdateDate = DateTime.Now;
            exchange.IsDeleted = 0;

            // save va luu vao database
            await _dataContext.Exchanges.AddAsync(exchange);
            await _dataContext.SaveChangesAsync();

            // chuyen ve dang response tra lai cho nguoi dung xem
            var exchangeResponseDTO = _mapper.Map<ExchangeResponseDTO>(exchange);
            return ServiceResponse<ExchangeResponseDTO>.Success(exchangeResponseDTO);
        }
        public async Task<ServiceResponse<ExchangeResponseDTO>> UpdateExchangeByIdAsync(int exchangeId, ExchangeRequestDTO exchangeRequestDTO)
        {
            // Lay user id
            var userId = int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            // Tao object de save tren server
            var exchange = await _dataContext.Exchanges.FirstOrDefaultAsync(e => e.Id == exchangeId);
            if (exchange == null)
            {
                return ServiceResponse<ExchangeResponseDTO>.Failure(400, "Exchange Not Found To Update");
            }
            // Update truong du lieu
            _mapper.Map(exchangeRequestDTO, exchange);
            exchange.UpdateBy = userId;
            exchange.UpdateDate = DateTime.Now;

            // save va luu vao database
            _dataContext.Exchanges.Update(exchange);
            await _dataContext.SaveChangesAsync();

            // chuyen ve dang response tra lai cho nguoi dung xem
            var exchangeResponseDTO = _mapper.Map<ExchangeResponseDTO>(exchange);
            return ServiceResponse<ExchangeResponseDTO>.Success(exchangeResponseDTO);
        }
        public async Task<ServiceResponse<ExchangeResponseDTO>> DeleteExchangeByIdAsync(int exchangeId)
        {
            var exchange = await _dataContext.Exchanges.FirstOrDefaultAsync(e => e.Id == exchangeId);
            if (exchange == null)
            {
                return ServiceResponse<ExchangeResponseDTO>.Failure(400, "Exchange is not exist");
            }
            exchange.IsDeleted = 1;
            exchange.UpdateDate = DateTime.Now;
            var exchangeResponseDto = _mapper.Map<ExchangeResponseDTO>(exchange);
            _dataContext.Exchanges.Update(exchange);
            await _dataContext.SaveChangesAsync();

            return ServiceResponse<ExchangeResponseDTO>.Success(exchangeResponseDto);
        }
    }
}
