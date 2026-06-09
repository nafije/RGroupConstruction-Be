using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class City : AuditedEntity<Guid>
{
    public string? Name { get; set; }
}
