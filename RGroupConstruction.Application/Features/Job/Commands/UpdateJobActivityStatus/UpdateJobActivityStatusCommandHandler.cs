using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Job.Commands.UpdateJobActivityStatus;

internal class UpdateJobActivityStatusCommandHandler(IJobService _JobService) : ICommandHandler<UpdateJobActivityStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateJobActivityStatusCommand request, CancellationToken cancellationToken)
        => await _JobService.UpdateJobActivityStatusAsync(request, cancellationToken);
}


