using RGroupConstruction.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RGroupConstruction.Domain.Entities;

public class User : IdentityUser, IAuditedEntity<string>
{
    [NotMapped]
    public string ID
    {
        get => Id;
        set => Id = value;
    }

    public Guid? CreatedBy { get; set; }
    public string? CreatedIP { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public string? DeletedIP { get; set; }
    public DateTime? DeletedOn { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? ModifiedBy { get; set; }
    public string? ModifiedIP { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

