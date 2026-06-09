using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Project.Queries.GetFeaturedProjects;

public class GetFeaturedProjectsQuery : IQuery<IEnumerable<ProjectDto>>
{
    public int Limit { get; set; } = 10;
}

