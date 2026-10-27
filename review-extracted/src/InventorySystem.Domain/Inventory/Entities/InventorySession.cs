using InventorySystem.Domain.Common;
using InventorySystem.Domain.Enums;

namespace InventorySystem.Domain.Entities;

public class InventorySession : BaseAuditableEntity
{
    public string SessionNumber { get; set; } = string.Empty;
    public InventoryType InventoryType { get; set; }
    public DateTime InventoryDate { get; set; }

    public int BranchId { get; set; }
    public int StoreId { get; set; }

    public string? BeforeFileName { get; set; }
    public string? AfterFileName { get; set; }

    public string Status { get; set; } = "Completed";

    public Branch? Branch { get; set; }
    public Store? Store { get; set; }

    public ICollection<InventoryDetail> Details { get; set; }
        = new List<InventoryDetail>();
}