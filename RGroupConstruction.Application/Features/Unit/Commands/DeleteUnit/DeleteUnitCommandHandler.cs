using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Unit.Commands.DeleteUnit;

internal class DeleteUnitCommandHandler(IUnitService _unitService) : ICommandHandler<DeleteUnitCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        => await _unitService.DeleteUnitAsync(request, cancellationToken);
}
