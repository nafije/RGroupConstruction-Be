using RGroupConstruction.Application.Common.Model;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Project.Commands.UpdateProject;

public record UpdateProjectCommand : ICommand<ProjectDto>
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public int Year { get; set; }
    public string? CityId { get; set; }
    public string? ProjectType { get; set; }
    public string? ProjectStatusId { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Facilities { get; set; }
    public int TotalUnits { get; set; }
    public int ResidentialUnits { get; set; }
    public int ComercialUnits { get; set; }
    public int ParkingUnits { get; set; }
    public string? ProjectPlanFileUrl { get; set; }
    public string? ProjectPlanFileName { get; set; }
    public List<ProjectImagesCommand>? ProjectImages { get; set; }
    public List<ProjectTranslationCommand> ProjectTranslations { get; set; } = [];
}

