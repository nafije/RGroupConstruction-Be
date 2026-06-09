using System.ComponentModel.DataAnnotations;

namespace RGroupConstruction.Domain.Common;

public class AuditedEntity<TIdentity> : AuditedEntity, IAuditedEntity<TIdentity>
{
    [Key] public TIdentity? Id { get; set; }
}
[Serializable]
public class AuditedEntity : IAuditedEntity
{
    public bool IsDeleted { get; set; }

    public Guid? CreatedBy { get; set; }

    public string? CreatedIP { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? DeletedBy { get; set; }

    public string? DeletedIP { get; set; }

    public DateTime? DeletedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public string? ModifiedIP { get; set; }

    public DateTime? ModifiedOn { get; set; }
}

