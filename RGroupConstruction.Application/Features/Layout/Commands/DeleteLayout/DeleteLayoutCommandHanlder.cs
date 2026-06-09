using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Layout.Commands.DeleteLayout;

internal class DeleteLayoutCommandHanlder(ILayoutService _layoutService) : ICommandHandler<DeleteLayoutCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteLayoutCommand request, CancellationToken cancellationToken)
        => await _layoutService.DeleteLayoutAsync(request, cancellationToken);
}



