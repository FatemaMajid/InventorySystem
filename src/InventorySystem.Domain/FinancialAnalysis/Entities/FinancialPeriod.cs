using InventorySystem.Domain.Common;
using InventorySystem.Domain.FinancialAnalysis.Enums;

namespace InventorySystem.Domain.FinancialAnalysis.Entities;

public class FinancialPeriod : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public FinancialPeriodStatus Status { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<FinancialUpload> FinancialUploads { get; set; }
        = new List<FinancialUpload>();
}
