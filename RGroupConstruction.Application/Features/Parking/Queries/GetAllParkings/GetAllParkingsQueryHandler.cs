using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.Parking.Queries.GetAllParkingUnits;

internal class GetAllParkingsQueryHandler(IParkingService _parkingService) : IQueryHandler<GetAllParkingsQuery, PagedResponse<ParkingDto>>
{
    public async Task<Result<PagedResponse<ParkingDto>>> Handle(GetAllParkingsQuery request, CancellationToken cancellationToken)
        => await _parkingService.GetAllParkingsAsync(request, cancellationToken);
}

