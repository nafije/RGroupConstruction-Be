using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Application.Queries;

public class GetAllPagedJobApplicationsQuery : PagedQuery, IQuery<PagedResponse<JobApplicationDto>>
{
    public string? JobId { get; set; }
}

