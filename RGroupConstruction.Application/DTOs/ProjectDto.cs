using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class ProjectDto : BaseDto<Guid>
{
    public string? Name { get; set; }
    public CityDto? City { get; set; }
    public string? ProjectType { get; set; }
    public StatusDto? ProjectStatus { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; } 
    public string? Facilities { get; set; } 
    public bool IsFeatured { get; set; }
    public int Year { get; set; }

    public int TotalUnits { get; set; }
    public int ResidentialUnits { get; set; }
    public int ComercialUnits { get; set; }
    public int ParkingUnits { get; set; }
    public int AvailableResidentialUnits { get; set; }
    public int AvailableCcomercialUnits { get; set; }
    public int AvailableParkingUnits { get; set; }
    public string? ProjectPlanFileUrl { get; set; }
    public string? ProjectPlanFileName { get; set; }

    public ICollection<ProjectImageDto>? ProjectImages { get; set; }
    public ICollection<UnitDto>? Units { get; set; }
    public List<ProjectTranslationDto>? ProjectTranslations { get; set; }

}

