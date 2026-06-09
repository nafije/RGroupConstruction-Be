using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Unit.Queries.GetAllUnits;

internal class GetAllUnitsQueryHandler(IUnitService _unitService) : IQueryHandler<GetAllUnitsQuery, IEnumerable<UnitDto>>
{
    public async Task<Result<IEnumerable<UnitDto>>> Handle(GetAllUnitsQuery request, CancellationToken cancellationToken)
        => await _unitService.GetAllUnitsAsync(request, cancellationToken);
}


