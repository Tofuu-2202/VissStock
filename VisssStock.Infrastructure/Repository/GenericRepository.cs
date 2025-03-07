using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models.Pagination;
using VisssStock.Infrastructure.Data;

namespace VisssStock.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private static readonly char[] CommaSeparator = new[] { ',' };
        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(DataContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteByGuidIdAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByIntIdAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            _context.SaveChanges();
        }

        public void DeleteRangeByGuidIds(List<Guid> ids)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(Guid))
            {
                throw new ArgumentException($"Type {typeof(T).Name} does not have a Guid property named 'Id'.");
            }

            var entitiesToDelete = _context.Set<T>().AsEnumerable()
                .Where(e => ids.Contains((Guid)idProperty.GetValue(e)));
            _context.Set<T>().RemoveRange(entitiesToDelete);
            _context.SaveChanges();
        }

        public void DeleteRangeByIntIds(List<int> ids)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(int))
            {
                throw new ArgumentException($"Type {typeof(T).Name} does not have an int property named 'Id'.");
            }

            var entitiesToDelete = _context.Set<T>().AsEnumerable()
                .Where(e => ids.Contains((int)idProperty.GetValue(e)));
            _context.Set<T>().RemoveRange(entitiesToDelete);
            _context.SaveChanges();
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetByGuidIdAsync(Guid id, string? includeProperties = null)
        {
            var query = _context.Set<T>().AsQueryable().AsNoTracking();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id) ?? throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with ID {id} not found.");
        }

        public async Task<T> GetByIntIdAsync(int id, string? includeProperties = null)
        {
            var query = _context.Set<T>().AsQueryable().AsNoTracking();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id) ?? throw new KeyNotFoundException($"Entity of type {typeof(T).Name} with ID {id} not found.");
        }

        public async Task<PagedListResponseDTO<T>> GetFilteredAsync(FilterCriteria<T> criteria)
        {
            var query = _context.Set<T>().AsQueryable().AsNoTracking();

            if (criteria.Filter != null)
            {
                query = criteria.Filter.Aggregate(query, (current, filter) => current.Where(filter));
            }

            if (criteria.OrderBy != null)
            {
                query = criteria.OrderBy(query);
            }

            if (!string.IsNullOrEmpty(criteria.SearchTerm) && criteria.SearchFields != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var searchExpression = criteria.SearchFields
                    .Select(field =>
                        Expression.Call(
                            Expression.Property(parameter, field),
                            typeof(string).GetMethod("Contains", new[] { typeof(string) }) ?? throw new InvalidOperationException("Method 'Contains' not found"),
                            Expression.Constant(criteria.SearchTerm)))
                    .Aggregate<Expression>(Expression.OrElse);

                query = query.Where(Expression.Lambda<Func<T, bool>>(searchExpression, parameter));
            }

            if (!string.IsNullOrEmpty(criteria.IncludeProperties))
            {
                foreach (var includeProperty in criteria.IncludeProperties.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            var totalCount = await query.CountAsync();

            criteria.PageIndex = Math.Max(1, criteria.PageIndex);
            criteria.PageSize = Math.Max(1, criteria.PageSize);

            // Lấy kết quả với phân trang
            var results = await query.Skip((criteria.PageIndex - 1) * criteria.PageSize)
                                     .Take(criteria.PageSize)
                                     .ToListAsync();

            // Tạo và trả về PagedListResponseDTO
            return new PagedListResponseDTO<T>(new PagedList<T>(results, totalCount, criteria.PageIndex, criteria.PageSize));
        }


        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            _context.SaveChanges();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().CountAsync(predicate);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
