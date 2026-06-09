using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Status.Commands.CreateStatus;

public class CreateStatusCommand : ICommand<StatusDto>
{
    public string? Name { get; set; }
}


