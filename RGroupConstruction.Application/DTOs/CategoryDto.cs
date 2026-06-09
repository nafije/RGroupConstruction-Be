using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class CategoryDto : BaseDto<Guid>
{
    public string? Name { get; set; }
}

