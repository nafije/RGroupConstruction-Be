using RGroupConstruction.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace RGroupConstruction.Domain.Entities;

public class RefreshToken : AuditedEntity<Guid>
{
    [MaxLength(500)]
    public string? Token { get; set; }
    [Required]
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    [MaxLength(100)]
    public string? RevokedBy { get; set; }
    public DateTime? RevokedAt { get; set; }
    [MaxLength(45)]
    public string? IPAddress { get; set; }
    [MaxLength(500)]
    public string? DeviceInfo { get; set; }
    public string? UserId { get; set; }
    public virtual User? User { get; set; }
}
