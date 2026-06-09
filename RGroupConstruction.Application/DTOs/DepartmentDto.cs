using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class DepartmentDto : BaseDto<Guid>
{
    public string? Name { get; set; }
}


