using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class Department : AuditedEntity<Guid>
{
    public string? Name { get; set; }
}


