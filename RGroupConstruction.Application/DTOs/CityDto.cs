using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class CityDto : BaseDto<Guid>
{
    public string? Name { get; set; }
}


