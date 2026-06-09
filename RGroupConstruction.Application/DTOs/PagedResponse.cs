using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.DTOs;

public class PagedResponse<T>
{
    public List<T>? Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedResponse() { }

    public PagedResponse(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}


public abstract class PageNumberPagedQuery<TDto> : IQuery<PagedResponse<TDto>>
{
    public int PageNumber { get; set; } = 1;
    public virtual int PageSize { get; set; } = 10;
}
