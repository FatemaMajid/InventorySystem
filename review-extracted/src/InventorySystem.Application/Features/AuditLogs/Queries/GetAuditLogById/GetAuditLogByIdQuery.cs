using MediatR;

namespace InventorySystem.Application.Features.AuditLogs.Queries.GetAuditLogById;

public record GetAuditLogByIdQuery(int Id) : IRequest<GetAuditLogByIdResponse>;