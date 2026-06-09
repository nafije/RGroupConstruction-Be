using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Job.Commands.UpdateJob;

internal class UpdateJobCommandHandler(IJobService _careerService) : ICommandHandler<UpdateJobCommand, JobDto>
{
    public async Task<Result<JobDto>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        => await _careerService.UpdateJobAsync(request, cancellationToken);
}

