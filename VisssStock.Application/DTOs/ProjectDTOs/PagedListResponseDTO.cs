using VisssStock.Application.Models.Pagination;

public class PagedListResponseDTO<T> where T : class
{
    public IEnumerable<T> Items { get; set; }
    public int CurrentPage { get; set; } //PageNumber 
    public int TotalPages { get; set; }
    public int PageSize { get; set; } //PageSize 
    public int TotalCount { get; set; } //TotalCount 
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedListResponseDTO(PagedList<T> pagedList)
    {
        Items = pagedList;
        CurrentPage = pagedList.CurrentPage;
        TotalPages = pagedList.TotalPages;
        PageSize = pagedList.PageSize;
        TotalCount = pagedList.TotalCount;
        //HasPrevious = pagedList.HasPrevious;
        //HasNext = pagedList.HasNext;
    }

    public PagedListResponseDTO(IEnumerable<T> items, int currentPage, int totalPages, int pageSize, int totalCount)
    {
        Items = items;
        CurrentPage = currentPage;
        TotalPages = totalPages;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}