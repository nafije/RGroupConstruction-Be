using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommand : ICommand<CategoryDto>
{
    public string? Name { get; set; }
}

