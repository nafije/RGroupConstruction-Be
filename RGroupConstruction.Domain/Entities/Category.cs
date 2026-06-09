using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class Category : AuditedEntity<Guid>
{
    public string? Name { get; set; }
}

