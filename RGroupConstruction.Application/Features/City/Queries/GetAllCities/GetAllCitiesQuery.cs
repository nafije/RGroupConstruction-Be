using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.City.Queries.GetAllCities;

public class GetAllCitiesQuery : IQuery<IEnumerable<CityDto>>;

