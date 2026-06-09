using RGroupConstruction.Domain.Common;
using RGroupConstruction.Domain.Enums;

namespace RGroupConstruction.Domain.Entities;

public class UnitClient : AuditedEntity<Guid>
{
    public virtual Unit? Unit { get; set; }
    public string? ClientFullName { get; set; }
    public string? PhoneNumber { get; set; }
    public UnitClientStatus? Status { get; set; }
    public PaymentType? PaymentType { get; set; }
    public decimal ClosingPriceM2 { get; set; }
    public decimal ClosinTotalPrice { get; set; }
    public DateTime? Date { get; set; }
    public string? Installments { get; set; } //json
    public string? Comment { get; set; }
}

