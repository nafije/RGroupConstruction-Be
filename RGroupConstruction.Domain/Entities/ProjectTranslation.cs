using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class ProjectTranslation : AuditedEntity<Guid>
{
    public Project? Project { get; set; }
    public string? LanguageCode { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Facilities { get; set; }
}

