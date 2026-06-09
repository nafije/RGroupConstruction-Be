using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Job.Commands.DeleteJob;

public class DeleteJobCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}

