using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Status.Commands.UpdateStatus;

public record UpdateStatusCommand : ICommand<StatusDto>
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

