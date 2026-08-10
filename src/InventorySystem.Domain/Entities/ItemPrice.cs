using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.Entities;

public class ItemPrice : BaseAuditableEntity
{
    public int ItemId { get; set; }

    public decimal CustomerPrice { get; set; }

    public decimal ConsumerPrice { get; set; }

    // Navigation Property
    public Item? Item { get; set; }

    // Price History
    public ICollection<ItemPriceHistory> History { get; set; }
        = new List<ItemPriceHistory>();
}