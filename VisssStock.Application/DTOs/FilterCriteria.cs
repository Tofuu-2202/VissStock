using System.Linq.Expressions;

namespace VisssStock.Application.DTOs
{
    public class FilterCriteria<T> where T : class
    {
        public List<Expression<Func<T, bool>>>? Filter { get; set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
        public List<string>? SearchFields { get; set; }
        public string? IncludeProperties { get; set; }
    }
}
