using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Unit.Commands.CreateUnit;

internal class CreateUnitCommandHandler(IUnitService _unitService) : ICommandHandler<CreateUnitCommand, UnitDto>
{
    public async Task<Result<UnitDto>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        => await _unitService.CreateUnitAsync(request, cancellationToken);
}


