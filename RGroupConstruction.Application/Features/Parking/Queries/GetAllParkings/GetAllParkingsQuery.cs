using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Parking.Queries.GetAllParkingUnits;

public class GetAllParkingsQuery : IQuery<PagedResponse<ParkingDto>>
{
    public int PageNr { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

