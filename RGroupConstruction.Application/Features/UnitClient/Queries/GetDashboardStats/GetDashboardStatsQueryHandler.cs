using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.UnitClient.Queries.GetDashboardStats;

internal class GetDashboardStatsQueryHandler(IUnitClientService _unitClientService) : IQueryHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        => await _unitClientService.GetDashboardStatsAsync(request, cancellationToken);
}

