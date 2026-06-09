using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Status.Commands.DeleteStatus;

internal class DeleteStatusCommandHandler(IStatusService _statusService) : ICommandHandler<DeleteStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
        => await _statusService.DeleteStatusAsync(request, cancellationToken);
}

