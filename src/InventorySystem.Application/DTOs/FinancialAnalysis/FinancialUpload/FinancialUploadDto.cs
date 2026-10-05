using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Application.DTOs.FinancialAnalysis.FinancialUpload;

public class FinancialUploadDto
{
    public int Id { get; set; }
    public int FinancialPeriodId { get; set; }
    public FinancialFileType FileType { get; set; }
    public int? BranchId { get; set; }
    public string? BranchCode { get; set; }
    public string? BranchNameArabic { get; set; }
    public FinancialGender? Gender { get; set; }
    public FinancialFileScope Scope { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public FinancialUploadStatus Status { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? UploadedBy { get; set; }
    public string? ValidationMessage { get; set; }
}
