using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.Entities;

public class ItemLocation : BaseAuditableEntity
{
    public int ItemId { get; set; }

    public int BranchId { get; set; }

    public int StoreId { get; set; }

    // Navigation Properties
    public Item? Item { get; set; }

    public Branch? Branch { get; set; }

    public Store? Store { get; set; }
}