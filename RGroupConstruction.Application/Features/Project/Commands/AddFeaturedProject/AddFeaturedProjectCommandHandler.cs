using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Project.Commands.AddFeaturedProject;

internal class AddFeaturedProjectCommandHandler(IProjectService _projectService) : ICommandHandler<AddFeaturedProjectCommand, bool>
{
    public async Task<Result<bool>> Handle(AddFeaturedProjectCommand request, CancellationToken cancellationToken)
        => await _projectService.AddFeaturedProjectAsync(request, cancellationToken);
}


