using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class ParkingDto : BaseDto<Guid>
{
    public virtual ProjectDto? Project { get; set; }
    public int FloorNr { get; set; }
    public int AvailableParking { get; set; }
    public int TotalFlorParking { get; set; }
    public int Price { get; set; }
}

