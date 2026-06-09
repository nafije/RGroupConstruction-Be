using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Status.Commands.DeleteStatus;

public class DeleteStatusCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}

