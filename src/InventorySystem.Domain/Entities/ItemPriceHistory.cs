using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.Entities;

public class ItemPriceHistory : BaseAuditableEntity
{
    public int ItemPriceId { get; set; }

    public decimal OldCustomerPrice { get; set; }
    public decimal NewCustomerPrice { get; set; }

    public decimal OldConsumerPrice { get; set; }
    public decimal NewConsumerPrice { get; set; }

    public DateTime ChangedAt { get; set; }

    public string ChangedBy { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;

    public ItemPrice? ItemPrice { get; set; }
}