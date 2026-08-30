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
        // =========================================
        // Active Sessions
        // Last 7 days regardless of status
        // =========================================

        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        var activeSessions = await _context.InventorySessions
            .AsNoTracking()
            .CountAsync(
                x =>
                    !x.IsDeleted &&
                    x.InventoryDate >= sevenDaysAgo,
                cancellationToken);


        // =========================================
        // Total Sessions
        // =========================================

        var totalSessions = await _context.InventorySessions
            .AsNoTracking()
            .CountAsync(
                x => !x.IsDeleted,
                cancellationToken);


        // =========================================
        // Recent Sessions
        // =========================================

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

                TotalItems = x.Details
                    .Count(d => !d.IsDeleted),

                TotalValue = x.Details
                    .Where(
                        d =>
                            !d.IsDeleted &&
                            d.AfterValue.HasValue)
                    .Sum(
                        d =>
                            d.AfterValue ?? 0)
            })
            .ToListAsync(cancellationToken);


        // =========================================
        // Overview
        // =========================================

        var branchesCount = await _context.Branches
            .AsNoTracking()
            .CountAsync(
                x =>
                    !x.IsDeleted &&
                    x.IsActive,
                cancellationToken);


        var storesCount = await _context.Stores
            .AsNoTracking()
            .CountAsync(
                x =>
                    !x.IsDeleted &&
                    x.IsActive,
                cancellationToken);


        var categoriesCount = await _context.Categories
            .AsNoTracking()
            .CountAsync(
                x =>
                    !x.IsDeleted &&
                    x.IsActive,
                cancellationToken);


        var itemsCount = await _context.Items
            .AsNoTracking()
            .CountAsync(
                x =>
                    !x.IsDeleted &&
                    x.IsActive,
                cancellationToken);


        // =========================================
        // Attention Items
        // Based on latest inventory session
        // =========================================

        var latestSessionId = await _context.InventorySessions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.InventoryDate)
            .ThenByDescending(x => x.Id)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);


        var attentionItems = 0;

        if (latestSessionId.HasValue)
        {
            var details = _context.InventoryDetails
                .AsNoTracking()
                .Where(
                    x =>
                        x.InventorySessionId == latestSessionId.Value &&
                        !x.IsDeleted &&
                        x.Item != null &&
                        !x.Item.IsDeleted);


            // Newly counted
            var newlyCounted = await details
                .CountAsync(
                    x =>
                        !x.QuantityBefore.HasValue &&
                        x.QuantityAfter.HasValue,
                    cancellationToken);


            // Fully depleted
            var fullyDepleted = await details
                .CountAsync(
                    x =>
                        x.QuantityBefore.HasValue &&
                        !x.QuantityAfter.HasValue,
                    cancellationToken);


            // Price changed
            var priceChanged = await details
                .CountAsync(
                    x =>
                        x.ConsumerPriceBefore.HasValue &&
                        x.ConsumerPriceAfter.HasValue &&
                        x.ConsumerPriceBefore !=
                        x.ConsumerPriceAfter,
                    cancellationToken);


            // Unit not defined
            var unitNotDefined = await details
                .CountAsync(
                    x =>
                        !x.Item!.UnitId.HasValue,
                    cancellationToken);


            attentionItems =
                newlyCounted +
                fullyDepleted +
                priceChanged +
                unitNotDefined;
        }


        // =========================================
        // Database Status
        // =========================================

        var databaseConnected = true;

        try
        {
            databaseConnected =
                await _context.Database
                    .CanConnectAsync(cancellationToken);
        }
        catch
        {
            databaseConnected = false;
        }


        // =========================================
        // Response
        // =========================================

        return new GetHomeDashboardResponse
        {
            User = new HomeUser
            {
                Name =
                    _currentUserService.Username
                    ?? string.Empty
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

                Database =
                    databaseConnected
                        ? "Connected"
                        : "Disconnected",

                Health =
                    databaseConnected
                        ? "Healthy"
                        : "Degraded"
            }
        };
    }
}