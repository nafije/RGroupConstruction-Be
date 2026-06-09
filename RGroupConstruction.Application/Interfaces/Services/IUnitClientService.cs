using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.UnitClient.Commands.CreateUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Commands.DeleteUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Commands.UpdateUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Queries.GetAllPagedUnitClients;
using RGroupConstruction.Application.Features.UnitClient.Queries.GetDashboardStats;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IUnitClientService
{
    Task<Result<UnitClientDto>> CreateUnitClientAsync(CreateUnitClientCommand request, CancellationToken cancellationToken = default);
    Task<Result<UnitClientDto>> UpdateUnitClientAsync(UpdateUnitClientCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteUnitClientAsync(DeleteUnitClientCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<UnitClientDto>>> GetAllPagedUnitClientsAsync(GetAllPagedUnitClientsQuery request, CancellationToken cancellationToken = default);
    Task<Result<DashboardStatsDto>> GetDashboardStatsAsync(GetDashboardStatsQuery request, CancellationToken cancellationToken = default);
}

