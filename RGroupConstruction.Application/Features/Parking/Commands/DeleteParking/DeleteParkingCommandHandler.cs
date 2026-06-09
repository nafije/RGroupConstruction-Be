using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.Interfaces.Command;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Parking.Commands.DeleteParkingUnit;

internal class DeleteParkingCommandHandler(IParkingService _parkingService) : ICommandHandler<DeleteParkingCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteParkingCommand request, CancellationToken cancellationToken)
        => await _parkingService.DeleteParkingAsync(request, cancellationToken);
}


