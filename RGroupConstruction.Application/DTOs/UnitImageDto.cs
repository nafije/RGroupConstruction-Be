using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class UnitImageDto : BaseDto<Guid>
{
    public string? ImageName { get; set; }
    public string? ImagePath { get; set; }
    public bool IsPrimary { get; set; }
}

