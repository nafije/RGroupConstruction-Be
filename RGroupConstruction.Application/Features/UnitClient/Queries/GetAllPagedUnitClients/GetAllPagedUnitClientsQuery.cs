using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.UnitClient.Queries.GetAllPagedUnitClients;

public class GetAllPagedUnitClientsQuery : PagedQuery, IQuery<PagedResponse<UnitClientDto>> { }

