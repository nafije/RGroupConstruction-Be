using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Project.Queries.GetAllPagedProjects;

internal class GetAllPagedProjectsQueryHandler(IProjectService _projectService) : IQueryHandler<GetAllPagedProjectsQuery, PagedResponse<ProjectDto>>
{
    public async Task<Result<PagedResponse<ProjectDto>>> Handle(GetAllPagedProjectsQuery request, CancellationToken cancellationToken)
        => await _projectService.GetAllPagedProjectsAsync(request, cancellationToken);
}

