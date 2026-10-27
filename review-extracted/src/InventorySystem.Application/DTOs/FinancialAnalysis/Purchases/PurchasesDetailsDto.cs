namespace InventorySystem.Application.DTOs.FinancialAnalysis.Purchases;

public sealed class PurchaseDetailRowDto
{
    public string AccountCode { get; set; } =
        string.Empty;

    public string AccountName { get; set; } =
        string.Empty;

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public decimal Balance { get; set; }

    public decimal Purchases { get; set; }

    public int RowOrder { get; set; }
}

public sealed class BranchPurchaseDetailsDto
{
    public int FinancialPeriodId { get; set; }

    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal TotalPurchases { get; set; }

    public List<PurchaseDetailRowDto> Details { get; set; } =
        new();
}
