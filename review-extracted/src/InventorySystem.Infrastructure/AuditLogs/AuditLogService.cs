using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Infrastructure.AuditLogs;

public sealed class AuditLogService : IAuditLogService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuditLogService(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task LogAsync(
        string action,
        string entity,
        string? entityId = null,
        string? details = null,
        string status = "Success",
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            UserId = _currentUser.UserId,
            UserName = _currentUser.Username ?? "System",
            Action = action,
            Entity = entity,
            EntityId = entityId,
            Details = details,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);
    }
}