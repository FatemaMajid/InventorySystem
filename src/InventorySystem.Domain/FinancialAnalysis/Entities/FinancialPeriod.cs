using InventorySystem.Domain.Common;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Domain.FinancialAnalysis.Entities;

public class FinancialPeriod : BaseAuditableEntity
{
    public int Year { get; set; }
    public int Month { get; set; }
    public FinancialPeriodStatus Status { get; set; } = FinancialPeriodStatus.Draft;
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }

    public ICollection<FinancialUpload> Uploads { get; set; }
        = new List<FinancialUpload>();
}
