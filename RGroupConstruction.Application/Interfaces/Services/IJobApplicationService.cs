using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Application.Commands.CreateJobApplication;
using RGroupConstruction.Application.Features.Application.Queries;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IJobApplicationService
{
    Task<Result<bool>> CreateJobApplicationAsync(CreateJobApplicationCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<JobApplicationDto>>> GetAllPagedJobApplicationsAsync(GetAllPagedJobApplicationsQuery request, CancellationToken cancellationToken = default);
}

