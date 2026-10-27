using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Settlements;

public sealed class SettlementGenderSummaryDto
{
    public bool HasFile { get; set; }

    public int? FinancialUploadId { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public decimal Balance { get; set; }
}

public sealed class BranchSettlementDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public SettlementGenderSummaryDto Male { get; set; } =
        new();

    public SettlementGenderSummaryDto Female { get; set; } =
        new();

    public bool HasAnyData =>
        Male.HasFile ||
        Female.HasFile;

    public bool HasCompleteData =>
        Male.HasFile &&
        Female.HasFile;

    public decimal TotalDebit =>
        Male.Debit +
        Female.Debit;

    public decimal TotalCredit =>
        Male.Credit +
        Female.Credit;

    public decimal TotalBalance =>
        Male.Balance +
        Female.Balance;
}

public sealed class SettlementBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public decimal TotalBalance { get; set; }
}

public sealed class SettlementsAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public int MaleFilesCount { get; set; }

    public int FemaleFilesCount { get; set; }

    public int BranchesWithAnyData { get; set; }

    public int BranchesWithCompleteData { get; set; }

    public int BranchesWithoutCompleteData { get; set; }

    public decimal AllBranchesMaleDebit { get; set; }

    public decimal AllBranchesMaleCredit { get; set; }

    public decimal AllBranchesMaleBalance { get; set; }

    public decimal AllBranchesFemaleDebit { get; set; }

    public decimal AllBranchesFemaleCredit { get; set; }

    public decimal AllBranchesFemaleBalance { get; set; }

    public decimal AllBranchesDebit { get; set; }

    public decimal AllBranchesCredit { get; set; }

    public decimal AllBranchesBalance { get; set; }

    public decimal AverageCompleteBranchBalance { get; set; }

    public SettlementBranchRankingDto? HighestBalanceBranch { get; set; }

    public SettlementBranchRankingDto? LowestBalanceBranch { get; set; }

    public List<BranchSettlementDto> Branches { get; set; } =
        new();
}

public sealed class SettlementTransactionDto
{
    public int Id { get; set; }

    public DateTime TransactionDate { get; set; }

    public string Description { get; set; } =
        string.Empty;

    public string? EntryNumber { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public decimal Balance { get; set; }

    public bool IsMarked { get; set; }

    public bool IsMatched { get; set; }

    public int RowOrder { get; set; }
}

public sealed class SettlementGenderDetailsDto
{
    public FinancialGender Gender { get; set; }

    public bool HasFile { get; set; }

    public int? FinancialUploadId { get; set; }

    public decimal PeriodDebit { get; set; }

    public decimal PeriodCredit { get; set; }

    public decimal PeriodBalance { get; set; }

    public int TransactionsCount { get; set; }

    public List<SettlementTransactionDto> Transactions { get; set; } =
        new();
}

public sealed class BranchSettlementDetailsDto
{
    public int FinancialPeriodId { get; set; }

    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public SettlementGenderDetailsDto Male { get; set; } =
        new();

    public SettlementGenderDetailsDto Female { get; set; } =
        new();

    public bool HasCompleteData =>
        Male.HasFile &&
        Female.HasFile;

    public decimal TotalDebit =>
        Male.PeriodDebit +
        Female.PeriodDebit;

    public decimal TotalCredit =>
        Male.PeriodCredit +
        Female.PeriodCredit;

    public decimal TotalBalance =>
        Male.PeriodBalance +
        Female.PeriodBalance;
}
