using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Interfaces.Command;

namespace RGroupConstruction.Application.Features.UnitClient.Commands.CreateUnitClient;

public class CreateUnitClientCommand : ICommand<UnitClientDto>
{
    public virtual string? UnitId { get; set; }
    public string? ClientFullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Status { get; set; }
    public string? PaymentType { get; set; }
    public decimal ClosingPriceM2 { get; set; }
    public decimal ClosinTotalPrice { get; set; }
    public DateTime? Date { get; set; }
    public string? Installments { get; set; } //json
    public string? Comment { get; set; }
}

