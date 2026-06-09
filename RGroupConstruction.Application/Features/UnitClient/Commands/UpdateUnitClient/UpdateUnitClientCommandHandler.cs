using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.UnitClient.Commands.UpdateUnitClient;

internal class UpdateUnitClientCommandHandler(IUnitClientService _unitClientService) : ICommandHandler<UpdateUnitClientCommand, UnitClientDto>
{
    public async Task<Result<UnitClientDto>> Handle(UpdateUnitClientCommand request, CancellationToken cancellationToken)
        => await _unitClientService.UpdateUnitClientAsync(request, cancellationToken);
}

