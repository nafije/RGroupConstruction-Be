using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Parking.Commands.UpdatePrakingUnit;

public record UpdatePrakingCommand : ICommand<ParkingDto>
{
    public string? Id { get; set; }
    public int FloorNr { get; set; }
    public int AvailableParking { get; set; }
    public int TotalFlorParking { get; set; }
    public int Price { get; set; }
}
