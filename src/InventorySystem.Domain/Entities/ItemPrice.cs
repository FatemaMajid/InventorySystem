using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.Entities;

public class ItemPrice : BaseAuditableEntity
{
    public int ItemId { get; set; }

    public decimal CustomerPrice { get; set; }
    public decimal ConsumerPrice { get; set; }

    public Item? Item { get; set; }

    public ICollection<ItemPriceHistory> History { get; set; }
        = new List<ItemPriceHistory>();
}