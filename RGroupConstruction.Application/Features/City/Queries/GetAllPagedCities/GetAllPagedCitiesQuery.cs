using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.City.Queries.GetAllPagedCities;

public class GetAllPagedCitiesQuery : PagedQuery, IQuery<PagedResponse<CityDto>> { }

