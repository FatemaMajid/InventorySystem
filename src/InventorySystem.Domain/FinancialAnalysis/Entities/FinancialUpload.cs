using InventorySystem.Domain.Common;
using InventorySystem.Domain.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Domain.FinancialAnalysis.Entities;

public class FinancialUpload : BaseAuditableEntity
{
    public int FinancialPeriodId { get; set; }
    public FinancialFileType FileType { get; set; }
    public int? BranchId { get; set; }
    public FinancialGender? Gender { get; set; }
    public FinancialFileScope Scope { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string FileHash { get; set; } = string.Empty;
    public long FileSize { get; set; }

    public FinancialUploadStatus Status { get; set; }
        = FinancialUploadStatus.Validating;

    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? UploadedBy { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public string? ValidationMessage { get; set; }

    public FinancialPeriod? FinancialPeriod { get; set; }
    public Branch? Branch { get; set; }

    public ICollection<FinancialAccountRow> AccountRows { get; set; }
        = new List<FinancialAccountRow>();

    public ICollection<TransferRecord> TransferRecords { get; set; }
        = new List<TransferRecord>();

    public SettlementSummary? SettlementSummary { get; set; }

    public ICollection<SettlementRow> SettlementRows { get; set; }
        = new List<SettlementRow>();
}
