using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Parking.Commands.UpdatePrakingUnit;

internal class UpdatePrakingCommandHandler(IParkingService _parkingService) : ICommandHandler<UpdatePrakingCommand, ParkingDto>
{
    public async Task<Result<ParkingDto>> Handle(UpdatePrakingCommand request, CancellationToken cancellationToken)
        => await _parkingService.UpdateParkingAsync(request, cancellationToken);
}


