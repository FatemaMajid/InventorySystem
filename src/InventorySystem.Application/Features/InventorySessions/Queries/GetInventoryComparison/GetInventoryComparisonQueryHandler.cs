using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparison;

public class GetInventoryComparisonQueryHandler
    : IRequestHandler<GetInventoryComparisonQuery, GetInventoryComparisonResponse>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryComparisonQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetInventoryComparisonResponse> Handle(
        GetInventoryComparisonQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.InventoryDetails
            .AsNoTracking()
            .Where(x =>
                x.InventorySessionId == request.SessionId &&
                !x.InventorySession!.IsDeleted &&
                !x.Item!.IsDeleted);

        query = ApplyFilters(query, request);

        var totalCount =
            await query.CountAsync(cancellationToken);

        query = ApplySorting(query, request);

        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new InventoryComparisonItem
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
                ? x.QuantityDifference.Value / x.QuantityBefore.Value * 100
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

        return new GetInventoryComparisonResponse
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static IQueryable<Domain.Entities.InventoryDetail> ApplyFilters(
        IQueryable<Domain.Entities.InventoryDetail> query,
        GetInventoryComparisonQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.ItemCode))
        {
            var itemCode = request.ItemCode.Trim();

            query = query.Where(x =>
                x.Item!.ItemCode.Contains(itemCode));
        }

        if (!string.IsNullOrWhiteSpace(request.ItemName))
        {
            var itemName = request.ItemName.Trim();

            query = query.Where(x =>
                x.Item!.ItemName1.Contains(itemName) ||
                x.Item.ItemName2.Contains(itemName));
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
            var status = request.Status.Trim();

            query = query.Where(x =>
                x.Status == status);
        }

        return query;
    }

    private static IQueryable<Domain.Entities.InventoryDetail> ApplySorting(
        IQueryable<Domain.Entities.InventoryDetail> query,
        GetInventoryComparisonQuery request)
    {
        var sortBy = request.SortBy
            .Trim()
            .ToLowerInvariant();

        return sortBy switch
        {
            "itemname" => request.Descending
                ? query.OrderByDescending(x => x.Item!.ItemName1)
                : query.OrderBy(x => x.Item!.ItemName1),

            "quantitybefore" => request.Descending
                ? query.OrderByDescending(x => x.QuantityBefore)
                : query.OrderBy(x => x.QuantityBefore),

            "quantityafter" => request.Descending
                ? query.OrderByDescending(x => x.QuantityAfter)
                : query.OrderBy(x => x.QuantityAfter),

            "difference" => request.Descending
                ? query.OrderByDescending(x => x.QuantityDifference)
                : query.OrderBy(x => x.QuantityDifference),

            "beforevalue" => request.Descending
                ? query.OrderByDescending(x => x.BeforeValue)
                : query.OrderBy(x => x.BeforeValue),

            "aftervalue" => request.Descending
                ? query.OrderByDescending(x => x.AfterValue)
                : query.OrderBy(x => x.AfterValue),

            "valuedifference" => request.Descending
                ? query.OrderByDescending(x => x.ValueDifference)
                : query.OrderBy(x => x.ValueDifference),

            "status" => request.Descending
                ? query.OrderByDescending(x => x.Status)
                : query.OrderBy(x => x.Status),

            _ => request.Descending
                ? query.OrderByDescending(x => x.Item!.ItemCode)
                : query.OrderBy(x => x.Item!.ItemCode)
        };
    }
}