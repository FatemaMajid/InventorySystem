using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsHandler : IRequestHandler<GetAuditLogsQuery, GetAuditLogsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetAuditLogsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAuditLogsResponse> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.AuditLogs
            .AsNoTracking()
            .AsQueryable();

        if (request.UserId.HasValue)
        {
            query = query.Where(x => x.UserId == request.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Action))
        {
            var action = request.Action.Trim();
            query = query.Where(x => x.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(request.Entity))
        {
            var entity = request.Entity.Trim();
            query = query.Where(x => x.Entity == entity);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            var dateTo = request.DateTo.Value.Date.AddDays(1);
            query = query.Where(x => x.CreatedAt < dateTo);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 300);

        var logs = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AuditLogListItem
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.UserName,
                Action = x.Action,
                Entity = x.Entity,
                EntityId = x.EntityId,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetAuditLogsResponse
        {
            Items = logs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}