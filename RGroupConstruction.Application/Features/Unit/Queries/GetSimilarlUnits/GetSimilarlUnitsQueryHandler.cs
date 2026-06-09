using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Unit.Queries.GetSimilarlUnits;

internal class GetSimilarlUnitsQueryHandler(IUnitService _unitService) : IQueryHandler<GetSimilarlUnitsQuery, IEnumerable<UnitDto>>
{
    public async Task<Result<IEnumerable<UnitDto>>> Handle(GetSimilarlUnitsQuery request, CancellationToken cancellationToken)
        => await _unitService.GetSimilarUnitsAsync(request, cancellationToken);
}

