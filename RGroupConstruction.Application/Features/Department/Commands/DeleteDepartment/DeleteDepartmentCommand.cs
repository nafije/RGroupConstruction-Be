using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Department.Commands.DeleteDepartment;

public class DeleteDepartmentCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}
