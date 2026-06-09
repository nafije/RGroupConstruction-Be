using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Layout.Commands.CreateLayout;

internal class CreateLayoutCommandHandler(ILayoutService _layoutService) : ICommandHandler<CreateLayoutCommand, LayoutDto>
{
    public async Task<Result<LayoutDto>> Handle(CreateLayoutCommand request, CancellationToken cancellationToken)
        => await _layoutService.CreateLayoutAsync(request, cancellationToken);
}


