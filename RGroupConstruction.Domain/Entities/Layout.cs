using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class Layout : AuditedEntity<Guid>
{
    public string? Name { get; set; }
}

