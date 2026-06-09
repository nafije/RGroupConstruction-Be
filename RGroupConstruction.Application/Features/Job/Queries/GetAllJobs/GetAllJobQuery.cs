using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Job.Queries.GetAllJobs;

public class GetAllJobsQuery : IQuery<IEnumerable<JobDto>>;

