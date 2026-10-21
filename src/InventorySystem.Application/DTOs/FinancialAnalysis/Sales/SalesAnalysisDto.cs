using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Sales;

public sealed class BranchSalesDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasIncomeStatement { get; set; }

    public decimal Sales { get; set; }

    public decimal SalesReturns { get; set; }

    public decimal DiscountAllowed { get; set; }

    public decimal BranchDiscountAllowed { get; set; }

    public decimal NetSales { get; set; }
}

public sealed class SalesBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal NetSales { get; set; }
}

public sealed class SalesAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public decimal AllBranchesSales { get; set; }

    public decimal AllBranchesSalesReturns { get; set; }

    public decimal AllBranchesDiscountAllowed { get; set; }

    public decimal AllBranchesBranchDiscountAllowed { get; set; }

    public decimal AllBranchesTotal { get; set; }

    public decimal AverageNetSales { get; set; }

    public SalesBranchRankingDto? HighestBranch { get; set; }

    public SalesBranchRankingDto? LowestBranch { get; set; }

    public List<BranchSalesDto> Branches { get; set; } =
        new();
}
