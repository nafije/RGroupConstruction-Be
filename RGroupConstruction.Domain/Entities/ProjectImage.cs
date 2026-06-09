using RGroupConstruction.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace RGroupConstruction.Domain.Entities;

public class ProjectImage : AuditedEntity<Guid>
{
    public virtual Project? Project { get; set; }

    [StringLength(255)]
    public string? ImageName { get; set; }

    [StringLength(255)]
    public string? ImagePath { get; set; }
    public bool IsPrimary { get; set; }
}

