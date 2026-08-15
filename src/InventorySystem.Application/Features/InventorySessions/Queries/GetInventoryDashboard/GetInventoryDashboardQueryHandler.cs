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

        var totalItems = await details.CountAsync(cancellationToken);

        var increase = await details.CountAsync(
            x => x.QuantityBefore.HasValue &&
                 x.QuantityAfter.HasValue &&
                 x.QuantityAfter > x.QuantityBefore,
            cancellationToken);

        var decrease = await details.CountAsync(
            x => x.QuantityBefore.HasValue &&
                 x.QuantityAfter.HasValue &&
                 x.QuantityAfter < x.QuantityBefore,
            cancellationToken);

        var match = await details.CountAsync(
            x => x.QuantityBefore.HasValue &&
                 x.QuantityAfter.HasValue &&
                 x.QuantityAfter == x.QuantityBefore,
            cancellationToken);

        var newlyCounted = await details.CountAsync(
            x => !x.QuantityBefore.HasValue &&
                 x.QuantityAfter.HasValue,
            cancellationToken);

        var fullyDepleted = await details.CountAsync(
            x => x.QuantityBefore.HasValue &&
                 !x.QuantityAfter.HasValue,
            cancellationToken);

        var priceChanged = await details.CountAsync(
            x => x.ConsumerPriceBefore.HasValue &&
                 x.ConsumerPriceAfter.HasValue &&
                 x.ConsumerPriceBefore != x.ConsumerPriceAfter,
            cancellationToken);

        var unitNotDefined = await details.CountAsync(
            x => !x.Item!.UnitId.HasValue,
            cancellationToken);

        var totalValueBefore =
            await details
                .Where(x => x.BeforeValue.HasValue)
                .SumAsync(
                    x => x.BeforeValue!.Value,
                    cancellationToken);

        var totalValueAfter =
            await details
                .Where(x => x.AfterValue.HasValue)
                .SumAsync(
                    x => x.AfterValue!.Value,
                    cancellationToken);

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
            ? (decimal)count / total * 100
            : 0;

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