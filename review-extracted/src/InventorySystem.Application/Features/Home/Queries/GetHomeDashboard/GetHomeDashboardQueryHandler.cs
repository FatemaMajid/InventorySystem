using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Home.Queries.GetHomeDashboard;

public sealed class GetHomeDashboardQueryHandler
    : IRequestHandler<GetHomeDashboardQuery, GetHomeDashboardResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetHomeDashboardQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<GetHomeDashboardResponse> Handle(
        GetHomeDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        var sessionStatistics = await _context.InventorySessions
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                ActiveSessions = g.Count(x =>
                    !x.IsDeleted &&
                    x.InventoryDate >= sevenDaysAgo),
                TotalSessions = g.Count(x => !x.IsDeleted)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var activeSessions = sessionStatistics?.ActiveSessions ?? 0;
        var totalSessions = sessionStatistics?.TotalSessions ?? 0;

        var recentSessions = await _context.InventorySessions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.InventoryDate)
            .ThenByDescending(x => x.Id)
            .Take(5)
            .Select(x => new HomeRecentSession
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
                TotalItems = x.Details.Count(d => !d.IsDeleted),
                TotalValue = x.Details
                    .Where(d =>
                        !d.IsDeleted &&
                        d.AfterValue.HasValue)
                    .Sum(d => d.AfterValue ?? 0)
            })
            .ToListAsync(cancellationToken);

        var branchesCount = await _context.Branches
            .AsNoTracking()
            .CountAsync(
                x => !x.IsDeleted && x.IsActive,
                cancellationToken);

        var storesCount = await _context.Stores
            .AsNoTracking()
            .CountAsync(
                x => !x.IsDeleted && x.IsActive,
                cancellationToken);

        var categoriesCount = await _context.Categories
            .AsNoTracking()
            .CountAsync(
                x => !x.IsDeleted && x.IsActive,
                cancellationToken);

        var itemsCount = await _context.Items
            .AsNoTracking()
            .CountAsync(
                x => !x.IsDeleted && x.IsActive,
                cancellationToken);

        var latestSessionId = recentSessions
            .Select(x => (int?)x.Id)
            .FirstOrDefault();

        var attentionItems = 0;

        if (latestSessionId.HasValue)
        {
            var details = _context.InventoryDetails
                .AsNoTracking()
                .Where(x =>
                    x.InventorySessionId == latestSessionId.Value &&
                    !x.IsDeleted &&
                    x.Item != null &&
                    !x.Item.IsDeleted);

            attentionItems = await details
                .Select(x =>
                    (x.QuantityBefore == null &&
                     x.QuantityAfter.HasValue ? 1 : 0)
                    +
                    (x.QuantityBefore.HasValue &&
                     x.QuantityAfter == null ? 1 : 0)
                    +
                    (!x.Item!.UnitId.HasValue ? 1 : 0)
                    +
                    (x.ConsumerPriceBefore.HasValue &&
                     x.ConsumerPriceAfter.HasValue &&
                     x.ConsumerPriceBefore != x.ConsumerPriceAfter
                        ? 1
                        : 0))
                .SumAsync(cancellationToken);
        }

        return new GetHomeDashboardResponse
        {
            User = new HomeUser
            {
                Name = _currentUserService.Username ?? string.Empty
            },
            Statistics = new HomeStatistics
            {
                ActiveSessions = activeSessions,
                TotalSessions = totalSessions,
                AttentionItems = attentionItems
            },
            RecentSessions = recentSessions,
            Overview = new HomeOverview
            {
                Branches = branchesCount,
                Stores = storesCount,
                Categories = categoriesCount,
                Items = itemsCount
            },
            SystemStatus = new HomeSystemStatus
            {
                Api = "Connected",
                Database = "Connected",
                Health = "Healthy"
            }
        };
    }
}