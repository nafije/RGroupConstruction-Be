using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Department.Commands.UpdateDepartment;

public record UpdateDepartmentCommand : ICommand<DepartmentDto>
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

