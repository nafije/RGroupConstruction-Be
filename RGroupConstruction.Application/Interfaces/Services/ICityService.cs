using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.City.Commands.CreateCity;
using RGroupConstruction.Application.Features.City.Commands.DeleteCity;
using RGroupConstruction.Application.Features.City.Commands.UpdateCity;
using RGroupConstruction.Application.Features.City.Queries.GetAllCities;
using RGroupConstruction.Application.Features.City.Queries.GetAllPagedCities;

namespace RGroupConstruction.Application.Interfaces.Services;

public interface ICityService
{
    Task<Result<CityDto>> CreateCityAsync(CreateCityCommand request, CancellationToken cancellationToken = default);
    Task<Result<CityDto>> UpdateCityAsync(UpdateCityCommand request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCityAsync(DeleteCityCommand request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<CityDto>>> GetAllPagedCitiesAsync(GetAllPagedCitiesQuery request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CityDto>>> GetAllCitiesAsync(GetAllCitiesQuery request, CancellationToken cancellationToken = default);
}

