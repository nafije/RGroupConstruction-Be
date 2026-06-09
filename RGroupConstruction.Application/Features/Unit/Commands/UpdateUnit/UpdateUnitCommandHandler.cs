using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Unit.Commands.UpdateUnit;

internal class UpdateUnitCommandHandler(IUnitService _unitService) : ICommandHandler<UpdateUnitCommand, UnitDto>
{
    public async Task<Result<UnitDto>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        => await _unitService.UpdateUnitAsync(request, cancellationToken);
}


