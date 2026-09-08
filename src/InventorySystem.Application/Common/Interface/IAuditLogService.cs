using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Common.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(
        string action,
        string entity,
        string? entityId = null,
        string? details = null,
        string status = "Success",
        CancellationToken cancellationToken = default);
}