using InventorySystem.Domain.Common;

namespace InventorySystem.Domain.Entities;

public class InventoryDetail : BaseAuditableEntity
{
    public int InventorySessionId { get; set; }

    public int ItemId { get; set; }

    // Quantities
    public decimal? QuantityBefore { get; set; }

    public decimal? QuantityAfter { get; set; }

    public decimal? QuantityDifference { get; set; }

    // Prices - Customer Price is used for inventory valuation
    public decimal? CustomerPriceBefore { get; set; }

    public decimal? CustomerPriceAfter { get; set; }

    // Values
    public decimal? BeforeValue { get; set; }

    public decimal? AfterValue { get; set; }

    public decimal? ValueDifference { get; set; }

    // Comparison Result
    public string Status { get; set; } = string.Empty;

    public string? Description { get; set; }

    // Navigation Properties
    public InventorySession? InventorySession { get; set; }

    public Item? Item { get; set; }
}