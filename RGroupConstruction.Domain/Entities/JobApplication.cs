using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class JobApplication : AuditedEntity<Guid>
{
    public Job? Job { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Message { get; set; }
    public string? CvFileUrl { get; set; }    
    public string? CvOriginalFileName { get; set; }
}

