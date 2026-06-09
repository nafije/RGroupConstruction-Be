namespace RGroupConstruction.Domain.Common;

public interface IAuditedEntity<TIdentity> : IAuditedEntity
{
    TIdentity Id { get; set; }
}
public interface IAuditedEntity
{
    Guid? CreatedBy { get; set; }
    string? CreatedIP { get; set; }
    DateTime? CreatedOn { get; set; }
    Guid? DeletedBy { get; set; }
    string? DeletedIP { get; set; }
    DateTime? DeletedOn { get; set; }
    bool IsDeleted { get; set; }
    Guid? ModifiedBy { get; set; }
    string? ModifiedIP { get; set; }
    DateTime? ModifiedOn { get; set; }
}


