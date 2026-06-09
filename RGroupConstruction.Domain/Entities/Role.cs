using RGroupConstruction.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace RGroupConstruction.Domain.Entities;


public class Role : IdentityRole, IAuditedEntity
{
    public Role(string? roleName,
        string? description) : base(roleName!)
    {
        Description = description;
    }

    public Role(string? roleName) : base(roleName!) { }

    public Role() { }

    public string? Description { get; set; }
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

