using AutoMapper;
using System.Linq.Expressions;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Utility;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Infrastructure.Repository.TransactionRepository
{
    public class TransactionRepository : GenericRepository<Transaction>
    {
        private readonly IMapper _mapper;
        private readonly Helper _helper;

        public TransactionRepository(DataContext context, IMapper mapper, Helper helper) : base(context)
        {
            _mapper = mapper; // Khởi tạo mapper
            _helper = helper;
        }

        public async Task<PagedListResponseDTO<TransactionResponseDto>> GetAllTransactionAsync(TransactionFillterDto filterDto)
        {

            var userId = _helper.GetCurrentUserId();
            var filterCriteria = new FilterCriteria<Transaction>
            {
                Filter = new List<Expression<Func<Transaction, bool>>>
                {
                    t => t.Quantity > 0,
                    t => t.UserId == userId
                },
                IncludeProperties = "Stock,Type"
            };

            if (filterDto.TypeId > 0)
            {
                filterCriteria.Filter.Add(t => t.TypeId == filterDto.TypeId);
            }

            if (filterDto.StockId > 0)
            {
                filterCriteria.Filter.Add(t => t.StockId == filterDto.StockId);
            }

            if (filterDto.FromQuantity > 0)
            {
                filterCriteria.Filter.Add(t =>
                    (filterDto.FromQuantity <= t.Quantity));
            }

            if (filterDto.ToQuantity > 0)
            {
                filterCriteria.Filter.Add(t =>
                    (filterDto.ToQuantity >= t.Quantity));
            }

            if (filterDto.FromPrice > 0)
            {
                filterCriteria.Filter.Add(t =>
                    (filterDto.FromPrice <= t.Price));
            }

            if (filterDto.ToPrice > 0)
            {
                filterCriteria.Filter.Add(t =>
                    (filterDto.ToPrice >= t.Price));
            }

            if (filterDto.FromTime > 0)
            {
                filterCriteria.Filter.Add(t =>
                    (filterDto.FromTime <= t.Time));
            }

            if (filterDto.ToTime > 0)
            {
                filterCriteria.Filter.Add(t =>
                    (filterDto.ToTime >= t.Time));
            }

            var transaction = await GetFilteredAsync(filterCriteria);

            var itemDtos = _mapper.Map<List<TransactionResponseDto>>(transaction.Items.ToList());

            var pagedList = new PagedListResponseDTO<TransactionResponseDto>(itemDtos, transaction.CurrentPage, transaction.TotalPages, transaction.PageSize, transaction.TotalCount);

            return pagedList;
        }

        //create transaction
        public async Task<TransactionResponseDto> CreateTransactionAsync(TransactionRequestDto transactionRequestDto)
        {
            var transaction = _mapper.Map<Transaction>(transactionRequestDto);
            transaction.UserId = _helper.GetCurrentUserId();
            //transaction.Time = _helper.GetUnixTime();

            await AddAsync(transaction);

            return _mapper.Map<TransactionResponseDto>(transaction);
        }

        //update transaction
        public async Task<TransactionResponseDto> UpdateTransactionAsync(int id, TransactionRequestDto transactionRequestDto)
        {
            var transaction = await GetByIntIdAsync(id);
            if (transaction == null)
            {
                throw new Exception("Transaction not found");
            }

            _mapper.Map(transactionRequestDto, transaction);
            //transaction.Time = _helper.GetUnixTime();

            await UpdateAsync(transaction);

            return _mapper.Map<TransactionResponseDto>(transaction);
        }

        //delete transaction
        public async Task DeleteTransactionAsync(int id)
        { 
            await DeleteByIntIdAsync(id);
        }
    }
}
