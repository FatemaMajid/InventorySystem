namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparison;

public class GetInventoryComparisonResponse
{
    public IReadOnlyList<InventoryComparisonItem> Items { get; init; } =
        Array.Empty<InventoryComparisonItem>();

    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public int TotalPages =>
        PageSize > 0
            ? (int)Math.Ceiling(
                (double)TotalCount / PageSize)
            : 0;
}

public class InventoryComparisonItem
{
    public int InventoryDetailId { get; init; }

    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName1 { get; init; } = string.Empty;
    public string? ItemName2 { get; init; }

    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;

    public int? UnitId { get; init; }
    public string UnitName { get; init; } = string.Empty;

    public decimal? QuantityBefore { get; init; }
    public decimal? QuantityAfter { get; init; }
    public decimal? QuantityDifference { get; init; }

    public decimal? ConsumerPriceBefore { get; init; }
    public decimal? ConsumerPriceAfter { get; init; }

    public decimal? BeforeValue { get; init; }
    public decimal? AfterValue { get; init; }
    public decimal? ValueDifference { get; init; }

    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
}