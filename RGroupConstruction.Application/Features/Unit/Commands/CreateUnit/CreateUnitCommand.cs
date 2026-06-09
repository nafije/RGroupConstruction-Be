using RGroupConstruction.Application.Common.Model;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.Unit.Commands.CreateUnit;

public class CreateUnitCommand : ICommand<UnitDto>
{
    public string? ProjectId { get; set; }
    public string? LayoutId { get; set; }
    public string? UnitCategoryId { get; set; }
    public string? UnitStatus { get; set; }
    public string? UnitRefNumber { get; set; }
    public int Floor { get; set; }
    public int BedRooms { get; set; }
    public int BathRooms { get; set; }
    public double NetArea { get; set; }
    public double GrossArea { get; set; }
    public decimal PriceM2 { get; set; }
    public decimal TotalPrice { get; set; }
    public double TerraceAre { get; set; }
    public string? FlorPlanFileUrl { get; set; }
    public string? FlorPlanFileName { get; set; }
    public List<UnitImagesCommand>? UnitImages { get; set; }
}

