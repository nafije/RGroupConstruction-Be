using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Unit.Commands.CreateUnit;
using RGroupConstruction.Application.Features.Unit.Commands.DeleteUnit;
using RGroupConstruction.Application.Features.Unit.Commands.UpdateUnit;
using RGroupConstruction.Application.Features.Unit.Queries.GetAllPagedUnits;
using RGroupConstruction.Application.Features.Unit.Queries.GetAllUnits;
using RGroupConstruction.Application.Features.Unit.Queries.GetSimilarlUnits;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IUnitService
{
    Task<Result<UnitDto>> CreateUnitAsync(CreateUnitCommand request, CancellationToken cancellationToken = default);
    Task<Result<UnitDto>> UpdateUnitAsync(UpdateUnitCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteUnitAsync(DeleteUnitCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<UnitDto>>> GetAllPagedUnitsAsync(GetAllPagedUnitsQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UnitDto>>> GetAllUnitsAsync(GetAllUnitsQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UnitDto>>> GetSimilarUnitsAsync(GetSimilarlUnitsQuery request, CancellationToken cancellationToken = default);
}

