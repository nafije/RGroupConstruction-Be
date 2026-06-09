using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Project.Queries.GetProjectById;

internal class GetProjectByIdQueryHandler(IProjectService _projectService) : IQueryHandler<GetProjectByIdQuery, ProjectDto>
{
    public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        => await _projectService.GetProjectByIdAsync(request, cancellationToken);
}

