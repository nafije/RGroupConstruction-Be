using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Project.Queries.GetAllPagedProjects;

public class GetAllPagedProjectsQuery : PagedQuery, IQuery<PagedResponse<ProjectDto>> 
{
    public string? ProjectType { get; set; }
    public string? ProjectStatusId { get; set; }
    public string? CityId { get; set; }
}
