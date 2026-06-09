using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Layout.Commands.UpdateLayout;

internal class UpdateLayoutCommandHandler(ILayoutService _layoutService) : ICommandHandler<UpdateLayoutCommand, LayoutDto>
{
    public async Task<Result<LayoutDto>> Handle(UpdateLayoutCommand request, CancellationToken cancellationToken)
        => await _layoutService.UpdateLayoutAsync(request, cancellationToken);
}



