using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Status.Queries.GetAllPagedStatuses;

internal class GetAllPagedStatusesQueryHandler(IStatusService _statusService) : IQueryHandler<GetAllPagedStatusesQuery, PagedResponse<StatusDto>>
{
    public async Task<Result<PagedResponse<StatusDto>>> Handle(GetAllPagedStatusesQuery request, CancellationToken cancellationToken)
        => await _statusService.GetAllPagedStatusesAsync(request, cancellationToken);
}
