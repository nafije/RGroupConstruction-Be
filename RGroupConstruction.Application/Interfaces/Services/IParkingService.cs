using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Parking.Commands.CreatePrakingUnit;
using RGroupConstruction.Application.Features.Parking.Commands.DeleteParkingUnit;
using RGroupConstruction.Application.Features.Parking.Commands.UpdatePrakingUnit;
using RGroupConstruction.Application.Features.Parking.Queries.GetAllParkingUnits;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface IParkingService
{
    Task<Result<PagedResponse<ParkingDto>>> GetAllParkingsAsync(GetAllParkingsQuery request, CancellationToken cancellationToken = default);
    Task<Result<ParkingDto>> CreateParkingAsync(CreatePrakingCommand request, CancellationToken cancellationToken = default);
    Task<Result<ParkingDto>> UpdateParkingAsync(UpdatePrakingCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteParkingAsync(DeleteParkingCommand request, CancellationToken cancellationToken = default);
}

