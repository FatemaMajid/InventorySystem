using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.ProfitLoss;

public sealed class BranchProfitLossDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasIncomeStatement { get; set; }

    public bool HasProfitLossResult { get; set; }

    public string ResultType { get; set; } =
        string.Empty;

    public decimal ResultValue { get; set; }

    public decimal SignedValue { get; set; }

    public string? SourceLabel { get; set; }
}

public sealed class ProfitLossBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public string ResultType { get; set; } =
        string.Empty;

    public decimal ResultValue { get; set; }

    public decimal SignedValue { get; set; }
}

public sealed class ProfitLossAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public int ProfitableBranches { get; set; }

    public int LossBranches { get; set; }

    public int BreakEvenBranches { get; set; }

    public decimal TotalProfit { get; set; }

    public decimal TotalLoss { get; set; }

    public decimal OverallResult { get; set; }

    public string OverallResultType { get; set; } =
        string.Empty;

    public decimal AverageResult { get; set; }

    public ProfitLossBranchRankingDto? HighestProfitBranch { get; set; }

    public ProfitLossBranchRankingDto? HighestLossBranch { get; set; }

    public List<BranchProfitLossDto> Branches { get; set; } =
        new();
}
