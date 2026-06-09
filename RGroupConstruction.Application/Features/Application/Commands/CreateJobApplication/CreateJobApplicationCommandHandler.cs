using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Application.Commands.CreateJobApplication;

internal class CreateJobApplicationCommandHandler(IJobApplicationService _jobApplicationService) : ICommandHandler<CreateJobApplicationCommand, bool>
{
    public async Task<Result<bool>> Handle(CreateJobApplicationCommand request, CancellationToken cancellationToken)
        => await _jobApplicationService.CreateJobApplicationAsync(request, cancellationToken);
}

