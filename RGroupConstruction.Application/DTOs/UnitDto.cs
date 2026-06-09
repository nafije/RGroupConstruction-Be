using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class UnitDto : BaseDto<Guid>
{
    public ProjectDto? Project { get; set; }
    public LayoutDto? Layout { get; set; }
    public CategoryDto? UnitCategory { get; set; }
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
    public ICollection<UnitImageDto>? UnitImages { get; set; }
}

