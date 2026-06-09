using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Log.Queries.GetAllPagedLogs;
using RGroupConstruction.Application.Features.Log.Queries.GetLogById;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface ILogService
{
    Task<Result<PagedResponse<LogEntryDto>>> GetAllPagedLogsAsync(GetAllPagedLogsQuery request, CancellationToken cancellationToken = default);
    Task<Result<LogEntryDto>> GetLogByIdAsync(GetLogByIdQuery request, CancellationToken cancellationToken = default);
}

