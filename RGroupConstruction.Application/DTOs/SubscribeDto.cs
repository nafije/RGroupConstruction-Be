using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class SubscribeDto : BaseDto<Guid>
{
    public string? Email { get; set; }
    public bool IsUnsubscribed { get; set; }
}

