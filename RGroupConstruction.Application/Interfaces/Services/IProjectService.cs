using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Project.Commands.AddFeaturedProject;
using RGroupConstruction.Application.Features.Project.Commands.CreateProject;
using RGroupConstruction.Application.Features.Project.Commands.DeleteProject;
using RGroupConstruction.Application.Features.Project.Commands.UpdateProject;
using RGroupConstruction.Application.Features.Project.Queries.GetAllPagedProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetAllProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetFeaturedProjects;
using RGroupConstruction.Application.Features.Project.Queries.GetProjectById;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IProjectService
{
    Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectCommand request, CancellationToken cancellationToken = default);
    Task<Result<ProjectDto>> UpdateProjectAsync(UpdateProjectCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> AddFeaturedProjectAsync(AddFeaturedProjectCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProjectAsync(DeleteProjectCommand request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProjectDto>>> GetFeaturedProjectsAsync(GetFeaturedProjectsQuery request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<ProjectDto>>> GetAllPagedProjectsAsync(GetAllPagedProjectsQuery request, CancellationToken cancellationToken = default);
    Task<Result<ProjectDto>> GetProjectByIdAsync(GetProjectByIdQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProjectDto>>> GetAllProjectsAsync(GetAllProjectsQuery request, CancellationToken cancellationToken = default);
}

