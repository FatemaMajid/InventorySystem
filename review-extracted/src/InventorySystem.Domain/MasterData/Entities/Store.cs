using InventorySystem.Domain.Common;
namespace InventorySystem.Domain.Entities;
public class Store : BaseAuditableEntity
{
    public string StoreCode { get; set; } = string.Empty;
    public string StoreNameArabic { get; set; } = string.Empty;
    public string? StoreNameEnglish { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    //Navigation Properties
    public Branch? Branch { get; set; }
}