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

                TotalQuantityBefore = g
                    .Where(x => x.QuantityBefore.HasValue)
                    .Sum(x => x.QuantityBefore!.Value),

                TotalQuantityAfter = g
                    .Where(x => x.QuantityAfter.HasValue)
                    .Sum(x => x.QuantityAfter!.Value),

                TotalValueBefore = g
                    .Where(x => x.BeforeValue.HasValue)
                    .Sum(x => x.BeforeValue!.Value),

                TotalValueAfter = g
                    .Where(x => x.AfterValue.HasValue)
                    .Sum(x => x.AfterValue!.Value)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var totalItems = stats?.TotalItems ?? 0;
        var increase = stats?.Increase ?? 0;
        var decrease = stats?.Decrease ?? 0;
        var match = stats?.Match ?? 0;
        var newlyCounted = stats?.NewlyCounted ?? 0;
        var fullyDepleted = stats?.FullyDepleted ?? 0;
        var priceChanged = stats?.PriceChanged ?? 0;
        var unitNotDefined = stats?.UnitNotDefined ?? 0;

        var totalQuantityBefore =
            stats?.TotalQuantityBefore ?? 0m;

        var totalQuantityAfter =
            stats?.TotalQuantityAfter ?? 0m;

        var totalValueBefore =
            stats?.TotalValueBefore ?? 0m;

        var totalValueAfter =
            stats?.TotalValueAfter ?? 0m;

        var quantityDifference =
            totalQuantityAfter - totalQuantityBefore;

        var quantityIncrease =
            increase > 0
                ? await details
                    .Where(x =>
                        x.QuantityBefore.HasValue &&
                        x.QuantityAfter.HasValue &&
                        x.QuantityAfter > x.QuantityBefore)
                    .SumAsync(
                        x => x.QuantityAfter!.Value - x.QuantityBefore!.Value,
                        cancellationToken)
                : 0m;

        var quantityDecrease =
            decrease > 0
                ? await details
                    .Where(x =>
                        x.QuantityBefore.HasValue &&
                        x.QuantityAfter.HasValue &&
                        x.QuantityAfter < x.QuantityBefore)
                    .SumAsync(
                        x => x.QuantityBefore!.Value - x.QuantityAfter!.Value,
                        cancellationToken)
                : 0m;

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

            InventoryQuantity = new InventoryQuantitySummary
            {
                TotalQuantityBefore = totalQuantityBefore,
                TotalQuantityAfter = totalQuantityAfter,
                QuantityDifference = quantityDifference,
                QuantityIncrease = quantityIncrease,
                QuantityDecrease = quantityDecrease
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
            ? (decimal)count / total * 100
            : 0;

        return new StatusSummary
        {
            Status = status,
            Count = count,
            Percentage = Math.Round(percentage,2)
        };
    }
}