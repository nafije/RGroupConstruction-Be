using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class RefreshTokenDto : BaseDto<Guid>
{
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string? RevokedBy { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? IPAddress { get; set; }
    public string? DeviceInfo { get; set; }
    public UserDto? User { get; set; }
}


