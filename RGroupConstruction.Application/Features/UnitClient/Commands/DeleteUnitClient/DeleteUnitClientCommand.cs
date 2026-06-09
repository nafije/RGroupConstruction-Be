using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.UnitClient.Commands.DeleteUnitClient;

public class DeleteUnitClientCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}

