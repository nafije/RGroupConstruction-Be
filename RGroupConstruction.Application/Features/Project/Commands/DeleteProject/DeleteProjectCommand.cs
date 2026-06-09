using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Project.Commands.DeleteProject;

public class DeleteProjectCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}

