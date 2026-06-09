namespace RGroupConstruction.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalClients { get; set; }
    public int TotalReservedUnits { get; set; }
    public int TotalSoldUnits { get; set; }
    public int TotalCashClients { get; set; }
    public int TotalInstallmentClients { get; set; }
    public string? MostSoldProject { get; set; }
    public List<ProjectDto>? Projects { get; set; }
}


