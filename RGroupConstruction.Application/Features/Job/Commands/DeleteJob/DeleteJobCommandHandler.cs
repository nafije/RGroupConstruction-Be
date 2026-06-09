using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Job.Commands.DeleteJob;

internal class DeleteJobCommandHandler(IJobService _careerService) : ICommandHandler<DeleteJobCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
        => await _careerService.DeleteJobAsync(request, cancellationToken);
}
