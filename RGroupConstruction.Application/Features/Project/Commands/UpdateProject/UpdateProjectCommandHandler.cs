using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Project.Commands.UpdateProject;

internal class UpdateProjectCommandHandler(IProjectService _projectService) : ICommandHandler<UpdateProjectCommand, ProjectDto>
{
    public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        => await _projectService.UpdateProjectAsync(request, cancellationToken);
}


