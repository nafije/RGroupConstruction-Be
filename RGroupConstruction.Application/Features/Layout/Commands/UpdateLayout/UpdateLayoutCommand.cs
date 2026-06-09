using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Layout.Commands.UpdateLayout;

public record UpdateLayoutCommand : ICommand<LayoutDto>
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}


