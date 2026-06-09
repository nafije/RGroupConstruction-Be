using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.UnitClient.Queries.GetAllPagedUnitClients;

internal class GetAllPagedUnitClientsQueryHandler(IUnitClientService _unitClientService) : IQueryHandler<GetAllPagedUnitClientsQuery, PagedResponse<UnitClientDto>>
{
    public async Task<Result<PagedResponse<UnitClientDto>>> Handle(GetAllPagedUnitClientsQuery request, CancellationToken cancellationToken)
        => await _unitClientService.GetAllPagedUnitClientsAsync(request, cancellationToken);
}

