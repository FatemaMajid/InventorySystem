using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.Transfers;

public sealed class BranchTransferDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public bool HasMaleTransfer { get; set; }

    public bool HasFemaleTransfer { get; set; }

    public bool HasCompleteData =>
        HasMaleTransfer &&
        HasFemaleTransfer;

    public decimal MaleTransfer { get; set; }

    public decimal FemaleTransfer { get; set; }

    public decimal TotalTransfer { get; set; }
}

public sealed class TransferBranchRankingDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public decimal MaleTransfer { get; set; }

    public decimal FemaleTransfer { get; set; }

    public decimal TotalTransfer { get; set; }
}

public sealed class TransfersAnalysisDto
{
    public int FinancialPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public int ActiveBranches { get; set; }

    public bool HasMaleTransferFile { get; set; }

    public bool HasFemaleTransferFile { get; set; }

    public int MaleRecordsCount { get; set; }

    public int FemaleRecordsCount { get; set; }

    public int BranchesWithAnyData { get; set; }

    public int BranchesWithCompleteData { get; set; }

    public int BranchesWithoutCompleteData { get; set; }

    public decimal AllBranchesMaleTransfers { get; set; }

    public decimal AllBranchesFemaleTransfers { get; set; }

    public decimal AllBranchesTotalTransfers { get; set; }

    public decimal AverageCompleteBranchTransfers { get; set; }

    public TransferBranchRankingDto? HighestTransferBranch { get; set; }

    public TransferBranchRankingDto? LowestTransferBranch { get; set; }

    public List<BranchTransferDto> Branches { get; set; } =
        new();
}

public sealed class TransferGenderDetailDto
{
    public int FinancialUploadId { get; set; }

    public string AccountCode { get; set; } =
        string.Empty;

    public string AccountName { get; set; } =
        string.Empty;

    public FinancialGender Gender { get; set; }

    public decimal Value { get; set; }

    public int RowOrder { get; set; }
}

public sealed class BranchTransferDetailsDto
{
    public int FinancialPeriodId { get; set; }

    public int BranchId { get; set; }

    public string BranchCode { get; set; } =
        string.Empty;

    public string BranchName { get; set; } =
        string.Empty;

    public TransferGenderDetailDto? Male { get; set; }

    public TransferGenderDetailDto? Female { get; set; }

    public bool HasCompleteData =>
        Male is not null &&
        Female is not null;

    public decimal MaleTransfer =>
        Male?.Value ?? 0;

    public decimal FemaleTransfer =>
        Female?.Value ?? 0;

    public decimal TotalTransfer =>
        MaleTransfer +
        FemaleTransfer;
}
