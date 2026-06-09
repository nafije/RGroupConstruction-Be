using RGroupConstruction.Application.Common.Model;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Job.Commands.UpdateJob;

public record UpdateJobCommand : ICommand<JobDto>
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? DepartmentId { get; set; }
    public string? EmploymentType { get; set; }
    public string? Location { get; set; }
    public string? Requirements { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<JobTranslationCommand> JobTranslations { get; set; } = [];
}

