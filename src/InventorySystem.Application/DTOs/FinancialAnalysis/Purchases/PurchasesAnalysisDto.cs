using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Purchases;

public sealed class BranchPurchasesDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasIncomeStatement { get; set; }

    public bool HasPurchasesAccount { get; set; }

    public decimal Purchases { get; set; }
}

public sealed class PurchasesBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal Purchases { get; set; }
}

public sealed class PurchasesAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public decimal AllBranchesTotal { get; set; }

    public decimal AveragePurchases { get; set; }

    public PurchasesBranchRankingDto? HighestBranch { get; set; }

    public PurchasesBranchRankingDto? LowestBranch { get; set; }

    public List<BranchPurchasesDto> Branches { get; set; } =
        new();
}
