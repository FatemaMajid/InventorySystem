using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Cash;

public sealed class BranchCashDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasFinancialPosition { get; set; }

    public bool HasCashGroup { get; set; }

    public string? CashGroupAccountCode { get; set; }

    public string? CashGroupAccountName { get; set; }

    public int CashboxesCount { get; set; }

    public decimal TotalCash { get; set; }
}

public sealed class CashBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal TotalCash { get; set; }
}

public sealed class CashAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int BranchesWithData { get; set; }

    public int BranchesWithoutData { get; set; }

    public decimal AllBranchesCash { get; set; }

    public decimal AverageCash { get; set; }

    public CashBranchRankingDto? HighestCashBranch { get; set; }

    public CashBranchRankingDto? LowestCashBranch { get; set; }

    public List<BranchCashDto> Branches { get; set; } =
        new();
}

public sealed class CashboxDetailDto
{
    public string AccountCode { get; set; } =
        string.Empty;

    public string AccountName { get; set; } =
        string.Empty;

    public decimal Value { get; set; }

    public int RowOrder { get; set; }
}

public sealed class BranchCashDetailsDto
{
    public int FinancialPeriodId { get; set; }

    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public string CashGroupAccountCode { get; set; } =
        string.Empty;

    public string CashGroupAccountName { get; set; } =
        string.Empty;

    public decimal TotalCash { get; set; }

    public int CashboxesCount { get; set; }

    public List<CashboxDetailDto> Cashboxes { get; set; } =
        new();
}
