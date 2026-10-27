using MediatR;

namespace InventorySystem.Application.Features.AuditLogs.Queries.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    int? UserId = null,
    string? Action = null,
    string? Entity = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null
) : IRequest<GetAuditLogsResponse>;