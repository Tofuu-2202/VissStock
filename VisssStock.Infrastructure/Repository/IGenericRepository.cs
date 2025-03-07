using System.Linq.Expressions;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models.Pagination;

namespace VisssStock.Infrastructure.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Delete(T entity);
        Task DeleteByGuidIdAsync(Guid id);
        Task DeleteByIntIdAsync(int id);
        void DeleteRange(IEnumerable<T> entities);
        void DeleteRangeByGuidIds(List<Guid> ids);
        void DeleteRangeByIntIds(List<int> ids);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByGuidIdAsync(Guid id, string? includeProperties = null);
        Task<T> GetByIntIdAsync(int id, string? includeProperties = null);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<PagedListResponseDTO<T>> GetFilteredAsync(FilterCriteria<T> criteria);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
    }
}
