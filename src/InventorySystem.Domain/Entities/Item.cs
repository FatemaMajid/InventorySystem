using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.Entities;

public class Item : BaseAuditableEntity
{
    public string ItemCode { get; set; } = string.Empty;

    public string ItemName1 { get; set; } = string.Empty;

    public string ItemName2 { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public int UnitId { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Category? Category { get; set; }

    public Unit? Unit { get; set; }

    public ICollection<ItemLocation> ItemLocations { get; set; }
        = new List<ItemLocation>();

    // public ICollection<ItemPriceHistory> PriceHistory { get; set; }
    //     = new List<ItemPriceHistory>();

    public ICollection<InventoryDetail> InventoryDetails { get; set; }
        = new List<InventoryDetail>();
}