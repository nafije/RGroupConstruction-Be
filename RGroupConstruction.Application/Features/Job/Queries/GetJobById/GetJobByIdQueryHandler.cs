using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Job.Queries.GetJobById;

internal class GetJobByIdQueryHandler(IJobService _careerService) : IQueryHandler<GetJobByIdQuery, JobDto>
{
    public async Task<Result<JobDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
        => await _careerService.GetJobByIdAsync(request, cancellationToken);
}

