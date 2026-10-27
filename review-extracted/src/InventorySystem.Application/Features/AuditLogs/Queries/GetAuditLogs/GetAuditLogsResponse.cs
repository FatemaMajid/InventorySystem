namespace InventorySystem.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsResponse
{
    public IReadOnlyList<AuditLogListItem> Items { get; init; } =
        Array.Empty<AuditLogListItem>();

    public int TotalCount { get; init; }

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalPages =>
        PageSize > 0
            ? (int)Math.Ceiling((double)TotalCount / PageSize)
            : 0;
}

public class AuditLogListItem
{
    public int Id { get; init; }

    public int? UserId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Action { get; init; } = string.Empty;

    public string Entity { get; init; } = string.Empty;

    public string? EntityId { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
}