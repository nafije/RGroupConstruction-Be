using RGroupConstruction.Domain.Common;
using RGroupConstruction.Domain.Enums;

namespace RGroupConstruction.Domain.Entities;

public class Unit : AuditedEntity<Guid>
{
    public virtual Project? Project { get; set; }
    public virtual Layout? Layout { get; set; }
    public virtual Category? UnitCategory { get; set; }
    public UnitStatus UnitStatus { get; set; }
    public string? UnitRefNumber { get; set; }
    public int Floor { get; set; }
    public int BedRooms { get; set; }
    public int BathRooms { get; set; }
    public double NetArea { get; set; }
    public double GrossArea { get; set; }
    public double TerraceAre { get; set; }
    public decimal PriceM2 { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FlorPlanFileUrl { get; set; }
    public string? FlorPlanFileName { get; set; }
    public ICollection<UnitImage>? UnitImages { get; set; }
}

