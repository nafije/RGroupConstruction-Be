using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Job.Queries.GetAllActiveJobs;

public class GetAllActiveJobsQuery : PagedQuery, IQuery<PagedResponse<JobDto>>
{
    public string? DepartmentId { get; set; }
    public string? EmploymentType { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
}

