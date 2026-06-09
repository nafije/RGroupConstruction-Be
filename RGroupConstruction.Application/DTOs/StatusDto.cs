using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class StatusDto : BaseDto<Guid>
{
    public string? Name { get; set; }
}


