using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Log.Queries.GetLogById;

internal class GetLogByIdQueryHandler(ILogService _logService) : ICommandHandler<GetLogByIdQuery, LogEntryDto>
{
    public async Task<Result<LogEntryDto>> Handle(GetLogByIdQuery request, CancellationToken cancellationToken)
        => await _logService.GetLogByIdAsync(request, cancellationToken);
}

