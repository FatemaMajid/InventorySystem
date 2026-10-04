using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.FinancialAnalysis.Entities;

public class SettlementRow : BaseEntity
{
    public int FinancialUploadId { get; set; }

    public DateTime? TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? EntryNumber { get; set; }

    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public decimal? Balance { get; set; }

    public bool IsMarked { get; set; }
    public bool IsMatched { get; set; }

    public int RowOrder { get; set; }

    public FinancialUpload? FinancialUpload { get; set; }
}
