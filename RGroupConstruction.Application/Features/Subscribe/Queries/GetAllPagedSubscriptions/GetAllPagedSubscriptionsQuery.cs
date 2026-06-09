using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Subscribe.Queries.GetAllPagedSubscriptions;

public class GetAllPagedSubscriptionsQuery : PagedQuery, IQuery<PagedResponse<SubscribeDto>> { }
