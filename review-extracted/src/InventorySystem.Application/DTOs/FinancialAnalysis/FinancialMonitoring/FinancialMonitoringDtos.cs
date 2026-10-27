using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.FinancialMonitoring;

public class FinancialPeriodSummaryDto
{
    public int FinancialPeriodId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public FinancialPeriodStatus Status { get; set; }
    public int ActiveBranches { get; set; }
    public int ExpectedFiles { get; set; }
    public int ValidFiles { get; set; }
    public int MissingFiles { get; set; }
    public decimal CompletionPercentage { get; set; }
}

public class MissingFinancialFileDto
{
    public int? BranchId { get; set; }
    public string? BranchCode { get; set; }
    public string? BranchNameArabic { get; set; }
    public FinancialFileType FileType { get; set; }
    public string FileTypeName { get; set; } = string.Empty;
    public FinancialGender? Gender { get; set; }
    public string? GenderName { get; set; }
    public FinancialFileScope Scope { get; set; }
}

public class FinancialUploadSlotDto
{
    public FinancialFileType FileType { get; set; }
    public FinancialGender? Gender { get; set; }
    public bool IsUploaded { get; set; }
    public int? UploadId { get; set; }
    public FinancialUploadStatus? Status { get; set; }
    public int? Version { get; set; }
    public string? OriginalFileName { get; set; }
    public string? ValidationMessage { get; set; }
}

public class FinancialBranchStatusDto
{
    public int BranchId { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string BranchNameArabic { get; set; } = string.Empty;
    public int UploadedFiles { get; set; }
    public int RequiredFiles { get; set; } = 4;
    public bool IsComplete { get; set; }
    public FinancialUploadSlotDto IncomeStatement { get; set; } = new();
    public FinancialUploadSlotDto FinancialPosition { get; set; } = new();
    public FinancialUploadSlotDto SettlementMale { get; set; } = new();
    public FinancialUploadSlotDto SettlementFemale { get; set; } = new();
}
