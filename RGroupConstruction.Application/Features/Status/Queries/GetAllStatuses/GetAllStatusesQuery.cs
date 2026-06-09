using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Status.Queries.GetAllStatuses;

public class GetAllStatusesQuery : IQuery<IEnumerable<StatusDto>>;
