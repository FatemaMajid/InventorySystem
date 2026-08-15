using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventorySessions;

public class GetInventorySessionsQueryHandler
    : IRequestHandler<GetInventorySessionsQuery, GetInventorySessionsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetInventorySessionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetInventorySessionsResponse> Handle(
        GetInventorySessionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.InventorySessions
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        query = ApplyFilters(query, request);

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, request);

        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var sessions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new InventorySessionListItem
            {
                Id = x.Id,
                SessionNumber = x.SessionNumber,
                InventoryType = x.InventoryType.ToString(),
                InventoryDate = x.InventoryDate,

                BranchId = x.BranchId,
                BranchName = x.Branch != null
                    ? x.Branch.BranchNameArabic
                    : string.Empty,

                StoreId = x.StoreId,
                StoreName = x.Store != null
                    ? x.Store.StoreNameArabic
                    : string.Empty,

                Status = x.Status,

                TotalItems = x.Details.Count(),

                TotalValue = x.Details
                    .Where(d => d.AfterValue.HasValue)
                    .Sum(d => d.AfterValue ?? 0)
            })
            .ToListAsync(cancellationToken);

        return new GetInventorySessionsResponse
        {
            Items = sessions,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static IQueryable<Domain.Entities.InventorySession> ApplyFilters(
        IQueryable<Domain.Entities.InventorySession> query,
        GetInventorySessionsQuery request)
    {
        if (request.BranchId.HasValue)
            query = query.Where(x =>
                x.BranchId == request.BranchId.Value);

        if (request.StoreId.HasValue)
            query = query.Where(x =>
                x.StoreId == request.StoreId.Value);

        if (request.InventoryType.HasValue)
            query = query.Where(x =>
                x.InventoryType == request.InventoryType.Value);

        if (request.DateFrom.HasValue)
            query = query.Where(x =>
                x.InventoryDate >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
        {
            var dateTo = request.DateTo.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.InventoryDate < dateTo);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var status = request.Status.Trim();

            query = query.Where(x =>
                x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(request.SessionNumber))
        {
            var sessionNumber = request.SessionNumber.Trim();

            query = query.Where(x =>
                x.SessionNumber.Contains(sessionNumber));
        }

        return query;
    }

    private static IQueryable<Domain.Entities.InventorySession> ApplySorting(
        IQueryable<Domain.Entities.InventorySession> query,
        GetInventorySessionsQuery request)
    {
        var sortBy = request.SortBy.Trim().ToLowerInvariant();

        return sortBy switch
        {
            "sessionnumber" => request.Descending
                ? query.OrderByDescending(x => x.SessionNumber)
                : query.OrderBy(x => x.SessionNumber),

            "branch" => request.Descending
                ? query.OrderByDescending(x => x.Branch!.BranchNameArabic)
                : query.OrderBy(x => x.Branch!.BranchNameArabic),

            "store" => request.Descending
                ? query.OrderByDescending(x => x.Store!.StoreNameArabic)
                : query.OrderBy(x => x.Store!.StoreNameArabic),

            "status" => request.Descending
                ? query.OrderByDescending(x => x.Status)
                : query.OrderBy(x => x.Status),

            _ => request.Descending
                ? query.OrderByDescending(x => x.InventoryDate)
                : query.OrderBy(x => x.InventoryDate)
        };
    }
}