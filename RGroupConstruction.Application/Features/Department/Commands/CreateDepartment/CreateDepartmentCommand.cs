using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Department.Commands.CreateDepartment;

public class CreateDepartmentCommand : ICommand<DepartmentDto>
{
    public string? Name { get; set; }
}

