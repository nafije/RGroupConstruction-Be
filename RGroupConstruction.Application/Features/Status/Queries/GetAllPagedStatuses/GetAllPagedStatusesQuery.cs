using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Status.Queries.GetAllPagedStatuses;

public class GetAllPagedStatusesQuery : PagedQuery, IQuery<PagedResponse<StatusDto>> { }

