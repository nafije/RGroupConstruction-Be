using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Status.Commands.UpdateStatus;

internal class UpdateStatusCommandHAndler(IStatusService _statusService) : ICommandHandler<UpdateStatusCommand, StatusDto>
{
    public async Task<Result<StatusDto>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        => await _statusService.UpdateStatusAsync(request, cancellationToken);
}
