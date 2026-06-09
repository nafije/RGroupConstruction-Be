using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Project.Commands.DeleteProject;

internal class DeleteProjectCommandHandler(IProjectService _projectService) : ICommandHandler<DeleteProjectCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        => await _projectService.DeleteProjectAsync(request, cancellationToken);
}

