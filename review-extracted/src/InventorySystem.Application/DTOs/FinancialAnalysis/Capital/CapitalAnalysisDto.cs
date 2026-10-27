using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Capital;

public sealed class BranchCapitalDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasFinancialPosition { get; set; }

    public bool HasCapitalAccount { get; set; }

    public string? CapitalAccountCode { get; set; }

    public string? CapitalAccountName { get; set; }

    public decimal Assets { get; set; }

    public decimal Liabilities { get; set; }

    public decimal Capital { get; set; }
}

public sealed class CapitalBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal Capital { get; set; }
}

public sealed class CapitalAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public decimal AllBranchesCapital { get; set; }

    public decimal AverageCapital { get; set; }

    public CapitalBranchRankingDto? HighestCapitalBranch { get; set; }

    public CapitalBranchRankingDto? LowestCapitalBranch { get; set; }

    public List<BranchCapitalDto> Branches { get; set; } =
        new();
}
