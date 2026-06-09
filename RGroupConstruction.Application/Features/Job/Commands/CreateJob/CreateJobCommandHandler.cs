using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Job.Commands.CreateJob;

internal class CreateJobCommandHandler(IJobService _careerService) : ICommandHandler<CreateJobCommand, JobDto>
{
    public async Task<Result<JobDto>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        => await _careerService.CreateJobAsync(request, cancellationToken);
}



