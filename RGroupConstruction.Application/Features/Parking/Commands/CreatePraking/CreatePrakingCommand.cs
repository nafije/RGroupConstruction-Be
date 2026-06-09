using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Parking.Commands.CreatePrakingUnit;

public class CreatePrakingCommand : ICommand<ParkingDto>
{
    public string? ProjectId { get; set; }
    public int FloorNr { get; set; }
    public int AvailableParking { get; set; }
    public int TotalFlorParking { get; set; }
    public int Price { get; set; }
}

