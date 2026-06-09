using RGroupConstruction.Domain.Common;

namespace RGroupConstruction.Domain.Entities;

public class Parking : AuditedEntity<Guid>
{
    public virtual Project? Project { get; set; }
    public int FloorNr { get; set; }
    public int AvailableParking { get; set; }
    public int TotalFlorParking { get; set; }
    public int Price { get; set; }

}

