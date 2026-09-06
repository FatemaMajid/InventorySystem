using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetAttentionItems;

public class GetAttentionItemsQueryHandler
    : IRequestHandler<GetAttentionItemsQuery, GetAttentionItemsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetAttentionItemsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAttentionItemsResponse> Handle(
        GetAttentionItemsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.InventoryDetails
            .AsNoTracking()
            .Where(x =>
                x.InventorySessionId == request.SessionId &&
                !x.InventorySession!.IsDeleted &&
                !x.Item!.IsDeleted);

        query = ApplyAttentionFilter(
            query,
            request.AttentionType);

        query = ApplyFilters(
            query,
            request);

        query = ApplySorting(
            query,
            request);

        var totalCount =
            await query.CountAsync(cancellationToken);

        var pageNumber =
            Math.Max(request.PageNumber, 1);

        var pageSize =
            request.PageSize <= 0
                ? 20
                : Math.Clamp(request.PageSize, 1, 300);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AttentionItem
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

                ConsumerPriceBefore =
                    x.ConsumerPriceBefore,

                ConsumerPriceAfter =
                    x.ConsumerPriceAfter,

                BeforeValue = x.BeforeValue,
                AfterValue = x.AfterValue,
                ValueDifference = x.ValueDifference,

                AttentionType =
                    x.QuantityBefore == null &&
                    x.QuantityAfter.HasValue
                        ? "NewlyCounted"

                    : x.QuantityBefore.HasValue &&
                      x.QuantityAfter == null
                        ? "FullyDepleted"

                    : !x.Item.UnitId.HasValue
                        ? "UnitNotDefined"

                    : x.ConsumerPriceBefore.HasValue &&
                      x.ConsumerPriceAfter.HasValue &&
                      x.ConsumerPriceBefore !=
                      x.ConsumerPriceAfter
                        ? "PriceChanged"

                    : string.Empty
            })
            .ToListAsync(cancellationToken);

        return new GetAttentionItemsResponse
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static IQueryable<Domain.Entities.InventoryDetail>
        ApplyAttentionFilter(
            IQueryable<Domain.Entities.InventoryDetail> query,
            string? attentionType)
    {
        if (string.IsNullOrWhiteSpace(attentionType))
        {
            return query.Where(x =>
                (
                    !x.QuantityBefore.HasValue &&
                    x.QuantityAfter.HasValue
                )
                ||
                (
                    x.QuantityBefore.HasValue &&
                    !x.QuantityAfter.HasValue
                )
                ||
                (
                    !x.Item!.UnitId.HasValue
                )
                ||
                (
                    x.ConsumerPriceBefore.HasValue &&
                    x.ConsumerPriceAfter.HasValue &&
                    x.ConsumerPriceBefore !=
                    x.ConsumerPriceAfter
                ));
        }

        return attentionType.Trim() switch
        {
            "NewlyCounted" =>
                query.Where(x =>
                    !x.QuantityBefore.HasValue &&
                    x.QuantityAfter.HasValue),

            "FullyDepleted" =>
                query.Where(x =>
                    x.QuantityBefore.HasValue &&
                    !x.QuantityAfter.HasValue),

            "UnitNotDefined" =>
                query.Where(x =>
                    !x.Item!.UnitId.HasValue),

            "PriceChanged" =>
                query.Where(x =>
                    x.ConsumerPriceBefore.HasValue &&
                    x.ConsumerPriceAfter.HasValue &&
                    x.ConsumerPriceBefore !=
                    x.ConsumerPriceAfter),

            _ => query.Where(x => false)
        };
    }

    private static IQueryable<Domain.Entities.InventoryDetail>
        ApplyFilters(
            IQueryable<Domain.Entities.InventoryDetail> query,
            GetAttentionItemsQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                x.Item!.ItemCode.Contains(search) ||
                x.Item.ItemName1.Contains(search) ||
                x.Item.ItemName2!.Contains(search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.Item!.CategoryId ==
                request.CategoryId.Value);
        }

        if (request.UnitId.HasValue)
        {
            query = query.Where(x =>
                x.Item!.UnitId ==
                request.UnitId.Value);
        }

        return query;
    }

    private static IQueryable<Domain.Entities.InventoryDetail>
        ApplySorting(
            IQueryable<Domain.Entities.InventoryDetail> query,
            GetAttentionItemsQuery request)
    {
        var sortBy = request.SortBy
            .Trim()
            .ToLowerInvariant();

        return sortBy switch
        {
            "itemname" => request.Descending
                ? query.OrderByDescending(
                    x => x.Item!.ItemName1)
                : query.OrderBy(
                    x => x.Item!.ItemName1),

            "quantitybefore" => request.Descending
                ? query.OrderByDescending(
                    x => x.QuantityBefore)
                : query.OrderBy(
                    x => x.QuantityBefore),

            "quantityafter" => request.Descending
                ? query.OrderByDescending(
                    x => x.QuantityAfter)
                : query.OrderBy(
                    x => x.QuantityAfter),

            "difference" => request.Descending
                ? query.OrderByDescending(
                    x => x.QuantityDifference)
                : query.OrderBy(
                    x => x.QuantityDifference),

            "valuedifference" => request.Descending
                ? query.OrderByDescending(
                    x => x.ValueDifference)
                : query.OrderBy(
                    x => x.ValueDifference),

            _ => request.Descending
                ? query.OrderByDescending(
                    x => x.Item!.ItemCode)
                : query.OrderBy(
                    x => x.Item!.ItemCode)
        };
    }
}