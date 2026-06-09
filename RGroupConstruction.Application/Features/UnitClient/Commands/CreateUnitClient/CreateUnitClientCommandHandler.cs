using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.UnitClient.Commands.CreateUnitClient;

internal class CreateUnitClientCommandHandler(IUnitClientService _unitClientService) : ICommandHandler<CreateUnitClientCommand, UnitClientDto>
{
    public async Task<Result<UnitClientDto>> Handle(CreateUnitClientCommand request, CancellationToken cancellationToken)
        => await _unitClientService.CreateUnitClientAsync(request, cancellationToken);
}

