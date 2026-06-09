namespace RGroupConstruction.Application.Common.Paging;

public abstract class PagedQuery
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    private int _pageNr = DefaultPageNumber;
    private int _pageSize = DefaultPageSize;

    public string? Search { get; set; }

    public int PageNr
    {
        get => _pageNr;
        set => _pageNr = value < DefaultPageNumber ? DefaultPageNumber : value;
    }

    public virtual int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? DefaultPageSize : Math.Min(value, MaxPageSize);
    }
}

