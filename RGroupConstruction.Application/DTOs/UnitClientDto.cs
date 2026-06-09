using RGroupConstruction.Application.DTOs.Base;

namespace RGroupConstruction.Application.DTOs;

public class UnitClientDto : BaseDto<Guid>
{
    public virtual UnitDto? Unit { get; set; }
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

