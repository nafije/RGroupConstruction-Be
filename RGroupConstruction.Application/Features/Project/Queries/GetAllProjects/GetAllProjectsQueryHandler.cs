using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Project.Queries.GetAllProjects;

internal class GetAllProjectsQueryHandler(IProjectService _projectService) : IQueryHandler<GetAllProjectsQuery, IEnumerable<ProjectDto>>
{
    public async Task<Result<IEnumerable<ProjectDto>>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        => await _projectService.GetAllProjectsAsync(request, cancellationToken);
}

