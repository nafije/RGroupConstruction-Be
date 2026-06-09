using RGroupConstruction.Application.Common.Paging;

namespace RGroupConstruction.UnitTests.Common.Paging;

public class PagedQueryTests
{
    [Fact]
    public void PageNr_WhenLessThanOne_NormalizesToOne()
    {
        var query = new TestPagedQuery { PageNr = -5 };

        Assert.Equal(PagedQuery.DefaultPageNumber, query.PageNr);
    }

    [Fact]
    public void PageSize_WhenTooLarge_CapsAtMaxPageSize()
    {
        var query = new TestPagedQuery { PageSize = PagedQuery.MaxPageSize + 1 };

        Assert.Equal(PagedQuery.MaxPageSize, query.PageSize);
    }

    [Fact]
    public void PageSize_WhenLessThanOne_UsesDefaultPageSize()
    {
        var query = new TestPagedQuery { PageSize = 0 };

        Assert.Equal(PagedQuery.DefaultPageSize, query.PageSize);
    }

    private sealed class TestPagedQuery : PagedQuery;
}

