namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventorySessions;

public class GetInventorySessionsResponse
{
    public IReadOnlyList<InventorySessionListItem> Items { get; init; } =
        Array.Empty<InventorySessionListItem>();

    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public int TotalPages =>
        PageSize > 0
            ? (int)Math.Ceiling((double)TotalCount / PageSize)
            : 0;
}

public class InventorySessionListItem
{
    public int Id { get; init; }
    public string SessionNumber { get; init; } = string.Empty;
    public string InventoryType { get; init; } = string.Empty;
    public DateTime InventoryDate { get; init; }

    public int BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;

    public int StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public int TotalItems { get; init; }
    public decimal TotalValue { get; init; }
}