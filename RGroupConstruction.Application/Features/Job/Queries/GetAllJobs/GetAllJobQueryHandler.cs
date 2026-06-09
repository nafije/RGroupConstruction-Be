using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Job.Queries.GetAllJobs;

internal class GetAllJobQueryHandler(IJobService _careerService) : IQueryHandler<GetAllJobsQuery, IEnumerable<JobDto>>
{
    public async Task<Result<IEnumerable<JobDto>>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        => await _careerService.GetAllJobsAsync(request, cancellationToken);
}

