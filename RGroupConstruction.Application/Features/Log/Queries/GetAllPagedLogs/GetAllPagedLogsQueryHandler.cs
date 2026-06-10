using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Log.Queries.GetAllPagedLogs;

internal class GetAllPagedLogsQueryHandler(ILogService _logService) : IQueryHandler<GetAllPagedLogsQuery, PagedResponse<LogEntryDto>>
{
    public async Task<Result<PagedResponse<LogEntryDto>>> Handle(GetAllPagedLogsQuery request, CancellationToken cancellationToken)
        => await _logService.GetAllPagedLogsAsync(request, cancellationToken);
}

