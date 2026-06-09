using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Application.Commands.CreateJobApplication;

public class CreateJobApplicationCommand : ICommand<bool>
{
    public string? JobId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Message { get; set; }
    public string? CvFileUrl { get; set; }
    public string? CvOriginalFileName { get; set; }
}

