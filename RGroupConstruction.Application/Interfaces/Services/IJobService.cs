using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Job.Commands.CreateJob;
using RGroupConstruction.Application.Features.Job.Commands.DeleteJob;
using RGroupConstruction.Application.Features.Job.Commands.UpdateJob;
using RGroupConstruction.Application.Features.Job.Commands.UpdateJobActivityStatus;
using RGroupConstruction.Application.Features.Job.Queries.GetAllActiveJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetAllJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetAllPagedJobs;
using RGroupConstruction.Application.Features.Job.Queries.GetJobById;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IJobService
{
    Task<Result<JobDto>> CreateJobAsync(CreateJobCommand request, CancellationToken cancellationToken = default);
    Task<Result<JobDto>> UpdateJobAsync(UpdateJobCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateJobActivityStatusAsync(UpdateJobActivityStatusCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteJobAsync(DeleteJobCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<JobDto>>> GetAllPagedJobsAsync(GetAllPagedJobsQuery request, CancellationToken cancellationToken = default);
    Task<Result<JobDto>> GetJobByIdAsync(GetJobByIdQuery request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<JobDto>>> GetAllActiveJobsAsync(GetAllActiveJobsQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<JobDto>>> GetAllJobsAsync(GetAllJobsQuery request, CancellationToken cancellationToken = default);
}

