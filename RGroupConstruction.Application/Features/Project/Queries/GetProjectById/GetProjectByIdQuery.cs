using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Project.Queries.GetProjectById;

public class GetProjectByIdQuery : PagedQuery, IQuery<ProjectDto>
{
    public string? ProjectId { get; set; }
}
