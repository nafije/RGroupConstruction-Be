using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Status.Commands.CreateStatus;

internal class CreateStatusCommandHandler(IStatusService _statusService) : ICommandHandler<CreateStatusCommand, StatusDto>
{
    public async Task<Result<StatusDto>> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
        => await _statusService.CreateStatusAsync(request, cancellationToken);
}

