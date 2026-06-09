using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Project.Queries.GetFeaturedProjects;

internal class GetFeaturedProjectsQueryHandler(IProjectService _projectService) : IQueryHandler<GetFeaturedProjectsQuery, IEnumerable<ProjectDto>>
{
    public async Task<Result<IEnumerable<ProjectDto>>> Handle(GetFeaturedProjectsQuery request, CancellationToken cancellationToken)
        => await _projectService.GetFeaturedProjectsAsync(request, cancellationToken);
}

