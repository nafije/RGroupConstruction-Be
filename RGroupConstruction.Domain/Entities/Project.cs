using RGroupConstruction.Domain.Common;
using RGroupConstruction.Domain.Enums;

namespace RGroupConstruction.Domain.Entities;

public class Project : AuditedEntity<Guid>
{
    public string? Name { get; set; }
    public virtual City? City { get; set; }
    public ProjectType ProjectType { get; set; } //TODO Class
    public virtual Status? ProjectStatus { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; } //json
    public string? Facilities { get; set; } //json
    public int TotalUnits { get; set; }
    public int ResidentialUnits { get; set; }
    public int ComercialUnits { get; set; }
    public int ParkingUnits { get; set; }
    public bool IsFeatured { get; set; }
    public int Year { get; set; }
    public string? ProjectPlanFileUrl { get; set; }
    public string? ProjectPlanFileName { get; set; }
    public ICollection<ProjectImage>? ProjectImages { get; set; }
    public ICollection<Unit>? Units { get; set; }
    public ICollection<ProjectTranslation>? ProjectTranslations { get; set; } = [];

}

