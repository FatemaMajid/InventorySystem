using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparisonExport;

public class GetInventoryComparisonExportQueryHandler
    : IRequestHandler<
        GetInventoryComparisonExportQuery,
        IReadOnlyList<InventoryComparisonExportItem>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryComparisonExportQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<InventoryComparisonExportItem>> Handle(
        GetInventoryComparisonExportQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.InventoryDetails
            .AsNoTracking()
            .Where(x =>
                x.InventorySessionId == request.SessionId &&
                !x.InventorySession!.IsDeleted &&
                !x.Item!.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.ItemCode))
        {
            var value = request.ItemCode.Trim();

            query = query.Where(x =>
                x.Item!.ItemCode.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(request.ItemName))
        {
            var value = request.ItemName.Trim();

            query = query.Where(x =>
                x.Item!.ItemName1.Contains(value) ||
                x.Item.ItemName2.Contains(value));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.Item!.CategoryId == request.CategoryId.Value);
        }

        if (request.UnitId.HasValue)
        {
            query = query.Where(x =>
                x.Item!.UnitId == request.UnitId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var value = request.Status.Trim();

            query = query.Where(x =>
                x.Status == value);
        }

        query = ApplySorting(
            query,
            request.SortBy,
            request.Descending);

        var sql = query.ToQueryString();
        Console.WriteLine(sql);

        return await query
            .Select(x => new InventoryComparisonExportItem
            {
                InventoryDetailId = x.Id,
                ItemId = x.ItemId,
                ItemCode = x.Item!.ItemCode,
                ItemName1 = x.Item.ItemName1,
                ItemName2 = x.Item.ItemName2,
                CategoryId = x.Item.CategoryId,
                CategoryName = x.Item.Category != null
                    ? x.Item.Category.CategoryNameArabic
                    : string.Empty,
                UnitId = x.Item.UnitId,
                UnitName = x.Item.Unit != null
                    ? x.Item.Unit.UnitNameArabic
                    : string.Empty,
                QuantityBefore = x.QuantityBefore,
                QuantityAfter = x.QuantityAfter,
                QuantityDifference = x.QuantityDifference,
                DifferencePercentage =
                    x.QuantityBefore.HasValue &&
                    x.QuantityBefore.Value != 0 &&
                    x.QuantityDifference.HasValue
                        ? x.QuantityDifference.Value /
                          x.QuantityBefore.Value * 100
                        : null,
                ConsumerPriceBefore = x.ConsumerPriceBefore,
                ConsumerPriceAfter = x.ConsumerPriceAfter,
                BeforeValue = x.BeforeValue,
                AfterValue = x.AfterValue,
                ValueDifference = x.ValueDifference,
                UnitNotDefined = !x.Item!.UnitId.HasValue,
                Status = x.Status,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Domain.Entities.InventoryDetail> ApplySorting(
        IQueryable<Domain.Entities.InventoryDetail> query,
        string sortBy,
        bool descending)
    {
        return sortBy.Trim().ToLowerInvariant() switch
        {
            "itemname" => descending
                ? query.OrderByDescending(x => x.Item!.ItemName1)
                : query.OrderBy(x => x.Item!.ItemName1),

            "quantitybefore" => descending
                ? query.OrderByDescending(x => x.QuantityBefore)
                : query.OrderBy(x => x.QuantityBefore),

            "quantityafter" => descending
                ? query.OrderByDescending(x => x.QuantityAfter)
                : query.OrderBy(x => x.QuantityAfter),

            "difference" => descending
                ? query.OrderByDescending(x => x.QuantityDifference)
                : query.OrderBy(x => x.QuantityDifference),

            "beforevalue" => descending
                ? query.OrderByDescending(x => x.BeforeValue)
                : query.OrderBy(x => x.BeforeValue),

            "aftervalue" => descending
                ? query.OrderByDescending(x => x.AfterValue)
                : query.OrderBy(x => x.AfterValue),

            "valuedifference" => descending
                ? query.OrderByDescending(x => x.ValueDifference)
                : query.OrderBy(x => x.ValueDifference),

            "status" => descending
                ? query.OrderByDescending(x => x.Status)
                : query.OrderBy(x => x.Status),

            _ => descending
                ? query.OrderByDescending(x => x.Item!.ItemCode)
                : query.OrderBy(x => x.Item!.ItemCode)
        };
    }
}

public class InventoryComparisonExportItem
{
    public int InventoryDetailId { get; init; }
    public int ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName1 { get; init; } = string.Empty;
    public string? ItemName2 { get; init; }
    public int? CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public int? UnitId { get; init; }
    public string UnitName { get; init; } = string.Empty;
    public decimal? QuantityBefore { get; init; }
    public decimal? QuantityAfter { get; init; }
    public decimal? QuantityDifference { get; init; }
    public decimal? ConsumerPriceBefore { get; init; }
    public decimal? ConsumerPriceAfter { get; init; }
    public decimal? BeforeValue { get; init; }
    public decimal? AfterValue { get; init; }
    public decimal? ValueDifference { get; init; }
    public decimal? DifferencePercentage { get; init; }
    public bool UnitNotDefined { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
}