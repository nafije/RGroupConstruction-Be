using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Job.Queries.GetAllPagedJobs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Career.Queries.GetAllPagedCareers;

internal class GetAllPagedJobsQueryHandler(IJobService _careerService) : IQueryHandler<GetAllPagedJobsQuery, PagedResponse<JobDto>>
{
    public async Task<Result<PagedResponse<JobDto>>> Handle(GetAllPagedJobsQuery request, CancellationToken cancellationToken)
        => await _careerService.GetAllPagedJobsAsync(request, cancellationToken);
}

