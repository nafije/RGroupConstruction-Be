using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Job.Queries.GetAllActiveJobs;

internal class GetAllActiveJobsQueryHandler(IJobService _careerService) : IQueryHandler<GetAllActiveJobsQuery, PagedResponse<JobDto>>
{
    public async Task<Result<PagedResponse<JobDto>>> Handle(GetAllActiveJobsQuery request, CancellationToken cancellationToken)
        => await _careerService.GetAllActiveJobsAsync(request, cancellationToken);
}

