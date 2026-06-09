using RGroupConstruction.Application.DTOs.Base;
using RGroupConstruction.Domain.Enums;

namespace RGroupConstruction.Application.DTOs;

public class UserNotificationDto : BaseDto<Guid>
{
    public string? UserId { get; set; }
    public UserDto? User { get; set; }
    public string? Message { get; set; }
    public bool IsRead { get; set; }
    public UserNotificationType? UserNotificationType { get; set; }
}

