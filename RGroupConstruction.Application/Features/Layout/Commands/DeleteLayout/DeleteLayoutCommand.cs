using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Layout.Commands.DeleteLayout;

public class DeleteLayoutCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}


