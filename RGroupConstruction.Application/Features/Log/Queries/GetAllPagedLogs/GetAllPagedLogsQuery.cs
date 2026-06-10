using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Log.Queries.GetAllPagedLogs;

public class GetAllPagedLogsQuery : PagedQuery, IQuery<PagedResponse<LogEntryDto>>
{
    public string? Level { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

