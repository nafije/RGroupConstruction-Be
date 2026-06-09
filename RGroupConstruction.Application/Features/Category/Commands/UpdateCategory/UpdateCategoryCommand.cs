using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Category.Commands.UpdateCategory;

public record UpdateCategoryCommand : ICommand<CategoryDto>
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

