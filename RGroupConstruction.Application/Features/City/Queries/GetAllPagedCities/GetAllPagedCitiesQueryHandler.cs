using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;
using RGroupConstruction.Application.Interfaces.Services;

namespace RGroupConstruction.Application.Features.City.Queries.GetAllPagedCities;

internal class GetAllPagedCitiesQueryHandler(ICityService _cityService) : IQueryHandler<GetAllPagedCitiesQuery, PagedResponse<CityDto>>
{
    public async Task<Result<PagedResponse<CityDto>>> Handle(GetAllPagedCitiesQuery request, CancellationToken cancellationToken)
        => await _cityService.GetAllPagedCitiesAsync(request, cancellationToken);
}
