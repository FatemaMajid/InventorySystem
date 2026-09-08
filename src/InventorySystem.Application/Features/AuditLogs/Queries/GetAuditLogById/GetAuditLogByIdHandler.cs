using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.AuditLogs.Queries.GetAuditLogById;

public class GetAuditLogByIdHandler : IRequestHandler<GetAuditLogByIdQuery, GetAuditLogByIdResponse>
{
    private readonly IApplicationDbContext _context;

    public GetAuditLogByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAuditLogByIdResponse> Handle(
        GetAuditLogByIdQuery request,
        CancellationToken cancellationToken)
    {
        var log = await _context.AuditLogs
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetAuditLogByIdResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.UserName,
                Action = x.Action,
                Entity = x.Entity,
                EntityId = x.EntityId,
                Status = x.Status,
                Details = x.Details,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return log ?? throw new KeyNotFoundException(
            $"Audit log with id {request.Id} was not found.");
    }
}