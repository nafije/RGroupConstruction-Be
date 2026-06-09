using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Job.Queries.GetJobById;

public class GetJobByIdQuery(string? id) : IQuery<JobDto>
{
    public string? Id { get; } = id;
}

