using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Layout.Commands.CreateLayout;

public class CreateLayoutCommand : ICommand<LayoutDto>
{
    public string? Name { get; set; }
}

