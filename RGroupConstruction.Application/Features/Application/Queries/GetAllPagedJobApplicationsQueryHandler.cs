using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Application.Queries;

internal class GetAllPagedJobApplicationsQueryHandler(IJobApplicationService _jobApplicationService) : IQueryHandler<GetAllPagedJobApplicationsQuery, PagedResponse<JobApplicationDto>>
{
    public async Task<Result<PagedResponse<JobApplicationDto>>> Handle(GetAllPagedJobApplicationsQuery request, CancellationToken cancellationToken)
        => await _jobApplicationService.GetAllPagedJobApplicationsAsync(request, cancellationToken);
}

