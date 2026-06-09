using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Unit.Queries.GetSimilarlUnits;

public class GetSimilarlUnitsQuery(string? unitId) : IQuery<IEnumerable<UnitDto>>
{
    public string? UnitId { get; set; } = unitId;
}

