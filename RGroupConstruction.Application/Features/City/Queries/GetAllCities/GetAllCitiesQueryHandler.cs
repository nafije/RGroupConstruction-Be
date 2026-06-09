using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.City.Queries.GetAllCities;

internal class GetAllCitiesQueryHandler(ICityService _cityService) : IQueryHandler<GetAllCitiesQuery, IEnumerable<CityDto>>
{
    public async Task<Result<IEnumerable<CityDto>>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
        => await _cityService.GetAllCitiesAsync(request, cancellationToken);
}

