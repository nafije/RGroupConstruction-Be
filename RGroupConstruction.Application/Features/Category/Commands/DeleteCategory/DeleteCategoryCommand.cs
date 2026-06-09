using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Category.Commands.DeleteCategory;

public class DeleteCategoryCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}

