using RGroupConstruction.Application.Common.Paging;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Query;

namespace RGroupConstruction.Application.Features.Unit.Queries.GetAllPagedUnits;

public class GetAllPagedUnitsQuery : PagedQuery, IQuery<PagedResponse<UnitDto>>
{
    public string? ProjectId { get; set; }
    public string? CategoryId { get; set; }
    public string? LayoutId { get; set; }
    public int MinPrixe { get; set; }
    public int MaxPrice { get; set; }
    public int MinGrosArea { get; set; }
    public int MaxGrosArea { get; set; }
}

