using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class LayoutDto : BaseDto<Guid>
{
    public string? Name { get; set; }
}

