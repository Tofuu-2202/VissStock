using Microsoft.EntityFrameworkCore;

namespace VisssStock.Application.Models.Pagination
{
    public class ServicePagination<T> : List<T>
    {
        public int PageIndex { get; private set; } = 1;
        public int TotalPages { get; private set; }

        public ServicePagination(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = Math.Max(pageIndex,1);
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<ServicePagination<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new ServicePagination<T>(items, count, pageIndex, pageSize);
        }
    }
}
