using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Debt;

public sealed class BranchDebtDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasFinancialPosition { get; set; }

    public bool HasMaleDebtAccount { get; set; }

    public bool HasFemaleDebtAccount { get; set; }

    public decimal MaleDebt { get; set; }

    public decimal FemaleDebt { get; set; }

    public decimal TotalDebt { get; set; }
}

public sealed class DebtBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal MaleDebt { get; set; }

    public decimal FemaleDebt { get; set; }

    public decimal TotalDebt { get; set; }
}

public sealed class BranchDebtAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public decimal AllBranchesMaleDebt { get; set; }

    public decimal AllBranchesFemaleDebt { get; set; }

    public decimal AllBranchesTotalDebt { get; set; }

    public decimal AverageDebt { get; set; }

    public DebtBranchRankingDto? HighestDebtBranch { get; set; }

    public DebtBranchRankingDto? LowestDebtBranch { get; set; }

    public List<BranchDebtDto> Branches { get; set; } =
        new();
}
