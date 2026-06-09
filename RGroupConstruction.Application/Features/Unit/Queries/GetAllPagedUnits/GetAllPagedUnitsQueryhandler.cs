using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Unit.Queries.GetAllPagedUnits;

internal class GetAllPagedUnitsQueryhandler(IUnitService _unitService) : IQueryHandler<GetAllPagedUnitsQuery, PagedResponse<UnitDto>>
{
    public async Task<Result<PagedResponse<UnitDto>>> Handle(GetAllPagedUnitsQuery request, CancellationToken cancellationToken)
        => await _unitService.GetAllPagedUnitsAsync(request, cancellationToken);
}

