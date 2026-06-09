using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.UnitClient.Commands.DeleteUnitClient;

internal class DeleteUnitClientCommandHandler(IUnitClientService _unitClientService) : ICommandHandler<DeleteUnitClientCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteUnitClientCommand request, CancellationToken cancellationToken)
        => await _unitClientService.DeleteUnitClientAsync(request, cancellationToken);
}

