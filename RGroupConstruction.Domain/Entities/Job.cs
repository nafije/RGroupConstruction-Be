using RGroupConstruction.Domain.Common;
using RGroupConstruction.Domain.Enums;

namespace RGroupConstruction.Domain.Entities;

public class Job : AuditedEntity<Guid>
{
    public string? Title { get; set; }
    public Department? Department { get; set; }
    public EmploymentType EmploymentType { get; set; }
    public string? Location { get; set; }
    public string? Requirements { get; set; } //json
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public ICollection<JobApplication>? Applications { get; set; }
    public ICollection<JobTranslation>? JobTranslations { get; set; } = [];
}

