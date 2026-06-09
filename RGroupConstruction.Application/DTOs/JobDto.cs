using RGroupConstruction.Application.DTOs.Base;
using RGroupConstruction.Domain.Entities;

namespace RGroupConstruction.Application.DTOs;

public class JobDto : BaseDto<Guid>
{
    public string? Title { get; set; }
    public DepartmentDto? Department { get; set; }
    public string? EmploymentType { get; set; }
    public string? Location { get; set; }
    public string? Requirements { get; set; } 
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int TotalApplicants { get; set; }
    public ICollection<JobApplicationDto>? Applications { get; set; }
    public List<JobTranslationDto>? JobTranslations { get; set; }
}

