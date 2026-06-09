using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Unit.Commands.DeleteUnit;

public class DeleteUnitCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}

