using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.FinancialAnalysis.Entities;

public class SettlementSummary : BaseEntity
{
    public int FinancialUploadId { get; set; }

    public decimal PeriodDebit { get; set; }
    public decimal PeriodCredit { get; set; }
    public decimal PeriodBalance { get; set; }

    public FinancialUpload? FinancialUpload { get; set; }
}
