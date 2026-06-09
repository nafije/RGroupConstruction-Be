using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Parking.Commands.DeleteParkingUnit;

public class DeleteParkingCommand(string? id) : ICommand<bool>
{
    public string? Id { get; } = id;
}

