using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.FinancialAnalysis.Entities;

public class FinancialAccountRow : BaseEntity
{
    public int FinancialUploadId { get; set; }

    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;

    public decimal? Debit { get; set; }
    public decimal? Credit { get; set; }
    public decimal? Balance { get; set; }

    public decimal? Assets { get; set; }
    public decimal? Liabilities { get; set; }

    public int RowOrder { get; set; }

    public FinancialUpload? FinancialUpload { get; set; }
}
