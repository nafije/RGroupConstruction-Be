using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Layout.Queries.GetAllLayouts;

public class GetAllLayoutsQuery : IQuery<IEnumerable<LayoutDto>>;
