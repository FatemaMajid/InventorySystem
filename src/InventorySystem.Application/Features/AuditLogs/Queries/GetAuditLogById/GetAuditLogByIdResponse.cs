namespace InventorySystem.Application.Features.AuditLogs.Queries.GetAuditLogById;

public class GetAuditLogByIdResponse
{
    public int Id { get; init; }

    public int? UserId { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Action { get; init; } = string.Empty;

    public string Entity { get; init; } = string.Empty;

    public string? EntityId { get; init; }

    public string Status { get; init; } = string.Empty;

    public string? Details { get; init; }

    public DateTime CreatedAt { get; init; }
}