using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Category.Commands.DeleteCategory;

internal class DeleteCategoryCommandHandler(ICategoryService _categoryService) : ICommandHandler<DeleteCategoryCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        => await _categoryService.DeleteCategoryAsync(request, cancellationToken);
}
