using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Project.Queries.GetAllProjects;

public class GetAllProjectsQuery : IQuery<IEnumerable<ProjectDto>>;

