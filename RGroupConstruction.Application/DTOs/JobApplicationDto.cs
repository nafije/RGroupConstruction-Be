using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class JobApplicationDto : BaseDto<Guid>
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Message { get; set; }
    public string? CvFileUrl { get; set; }
    public string? CvOriginalFileName { get; set; }
    public JobDto? Job { get; set; }
}

