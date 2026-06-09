using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Status.Queries.GetAllStatuses;

internal class GetAllStatusesQueryHandler(IStatusService _statusService) : IQueryHandler<GetAllStatusesQuery, IEnumerable<StatusDto>>
{
    public async Task<Result<IEnumerable<StatusDto>>> Handle(GetAllStatusesQuery request, CancellationToken cancellationToken)
        => await _statusService.GetAllStatusesAsync(request, cancellationToken);
}

