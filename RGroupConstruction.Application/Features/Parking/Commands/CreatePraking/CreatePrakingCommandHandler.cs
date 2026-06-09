using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Parking.Commands.CreatePrakingUnit;

internal class CreatePrakingCommandHandler(IParkingService _parkingService) : ICommandHandler<CreatePrakingCommand, ParkingDto>
{
    public async Task<Result<ParkingDto>> Handle(CreatePrakingCommand request, CancellationToken cancellationToken)
        => await _parkingService.CreateParkingAsync(request, cancellationToken);
}


