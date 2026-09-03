
using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryDashboard;

public class GetInventoryDashboardQueryHandler
    : IRequestHandler<GetInventoryDashboardQuery, GetInventoryDashboardResponse>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryDashboardQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetInventoryDashboardResponse> Handle(
        GetInventoryDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _context.InventorySessions
            .AsNoTracking()
            .Include(x => x.Branch)
            .Include(x => x.Store)
            .FirstOrDefaultAsync(
                x => x.Id == request.SessionId && !x.IsDeleted,
                cancellationToken);

        if (session == null)
            throw new KeyNotFoundException(
                $"Inventory session {request.SessionId} was not found.");

        var details = _context.InventoryDetails
            .AsNoTracking()
            .Where(x =>
                x.InventorySessionId == request.SessionId &&
                !x.Item!.IsDeleted);

        // Single grouped aggregation instead of ~9 separate round-trips.
        // EF Core translates every branch below into one SQL statement
        // with conditional (CASE WHEN) aggregates.
        var stats = await details
            .GroupBy(x => 1)
            .Select(g => new
            {
                TotalItems = g.Count(),

                Increase = g.Count(x =>
                    x.QuantityBefore.HasValue &&
                    x.QuantityAfter.HasValue &&
                    x.QuantityAfter > x.QuantityBefore),

                Decrease = g.Count(x =>
                    x.QuantityBefore.HasValue &&
                    x.QuantityAfter.HasValue &&
                    x.QuantityAfter < x.QuantityBefore),

                Match = g.Count(x =>
                    x.QuantityBefore.HasValue &&
                    x.QuantityAfter.HasValue &&
                    x.QuantityAfter == x.QuantityBefore),

                NewlyCounted = g.Count(x =>
                    !x.QuantityBefore.HasValue &&
                    x.QuantityAfter.HasValue),

                FullyDepleted = g.Count(x =>
                    x.QuantityBefore.HasValue &&
                    !x.QuantityAfter.HasValue),

                PriceChanged = g.Count(x =>
                    x.ConsumerPriceBefore.HasValue &&
                    x.ConsumerPriceAfter.HasValue &&
                    x.ConsumerPriceBefore != x.ConsumerPriceAfter),

                UnitNotDefined = g.Count(x =>
                    !x.Item!.UnitId.HasValue),

                TotalValueBefore = g
                    .Where(x => x.BeforeValue.HasValue)
                    .Sum(x => x.BeforeValue!.Value),

                TotalValueAfter = g
                    .Where(x => x.AfterValue.HasValue)
                    .Sum(x => x.AfterValue!.Value)
            })
            .FirstOrDefaultAsync(cancellationToken);

        // FirstOrDefaultAsync returns null when the session has zero
        // detail rows (e.g. import failed partway) — the original code
        // would have returned all zeros in that case too, via Count
        // queries on an empty set, so we preserve that behavior here.
        var totalItems = stats?.TotalItems ?? 0;
        var increase = stats?.Increase ?? 0;
        var decrease = stats?.Decrease ?? 0;
        var match = stats?.Match ?? 0;
        var newlyCounted = stats?.NewlyCounted ?? 0;
        var fullyDepleted = stats?.FullyDepleted ?? 0;
        var priceChanged = stats?.PriceChanged ?? 0;
        var unitNotDefined = stats?.UnitNotDefined ?? 0;
        var totalValueBefore = stats?.TotalValueBefore ?? 0m;
        var totalValueAfter = stats?.TotalValueAfter ?? 0m;

        var totalDifference =
            totalValueAfter - totalValueBefore;

        var differencePercentage =
            totalValueBefore != 0
                ? totalDifference / totalValueBefore * 100
                : 0;

        var statusData = new List<StatusSummary>
        {
            CreateStatus(
                "Increase",
                increase,
                totalItems),

            CreateStatus(
                "Decrease",
                decrease,
                totalItems),

            CreateStatus(
                "Match",
                match,
                totalItems),

            CreateStatus(
                "NewlyCounted",
                newlyCounted,
                totalItems),

            CreateStatus(
                "FullyDepleted",
                fullyDepleted,
                totalItems)
        };

        var topValueDifferences = await details
            .Where(x => x.ValueDifference.HasValue)
            .OrderByDescending(
                x => Math.Abs(x.ValueDifference!.Value))
            .Take(10)
            .Select(x => new ValueDifferenceItem
            {
                ItemCode = x.Item!.ItemCode,
                ItemName = x.Item.ItemName1,
                ValueDifference = x.ValueDifference!.Value
            })
            .ToListAsync(cancellationToken);

        var attentionTotal =
            newlyCounted +
            fullyDepleted +
            unitNotDefined +
            priceChanged;

        return new GetInventoryDashboardResponse
        {
            Session = new SessionInfo
            {
                SessionId = session.Id,
                SessionNumber = session.SessionNumber,
                Status = session.Status,
                InventoryType = session.InventoryType.ToString(),
                InventoryDate = session.InventoryDate,

                BranchId = session.BranchId,
                BranchName = session.Branch?.BranchNameArabic ?? string.Empty,

                StoreId = session.StoreId,
                StoreName = session.Store?.StoreNameArabic ?? string.Empty,

                BeforeFileName = session.BeforeFileName,
                AfterFileName = session.AfterFileName
            },

            Summary = new DashboardSummary
            {
                TotalItems = totalItems,
                Increase = increase,
                Decrease = decrease,
                Match = match,
                NewlyCounted = newlyCounted,
                FullyDepleted = fullyDepleted,
                PriceChanged = priceChanged,
                UnitNotDefined = unitNotDefined
            },

            Financial = new FinancialSummary
            {
                TotalValueBefore = totalValueBefore,
                TotalValueAfter = totalValueAfter,
                TotalDifference = totalDifference,
                DifferencePercentage = differencePercentage
            },

            Statuses = statusData,

            TopValueDifferences = topValueDifferences,

            Attention = new AttentionSummary
            {
                Total = attentionTotal,
                NewlyCounted = newlyCounted,
                FullyDepleted = fullyDepleted,
                UnitNotDefined = unitNotDefined,
                PriceChanged = priceChanged
            }
        };
    }

    private static StatusSummary CreateStatus(
        string status,
        int count,
        int total)
    {
        var percentage = total > 0
            ? (decimal)count / total * 100 : 0;

        return new StatusSummary
        {
            Status = status,
            Count = count,
            Percentage = Math.Round(
                percentage,
                2)
        };
    }
}