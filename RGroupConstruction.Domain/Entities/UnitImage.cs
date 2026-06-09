using RGroupConstruction.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace RGroupConstruction.Domain.Entities;

public class UnitImage : AuditedEntity<Guid>
{
    public virtual Unit? Unit { get; set; }

    [StringLength(255)]
    public string? ImageName { get; set; }

    [StringLength(255)]
    public string? ImagePath { get; set; }
    public bool IsPrimary { get; set; }
}

