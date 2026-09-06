using MediatR;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetAttentionItems;

public record GetAttentionItemsQuery(
    int SessionId,
    string? AttentionType = null,
    string? Search = null,
    int? CategoryId = null,
    int? UnitId = null,
    int PageNumber = 1,
    int PageSize = 20,
    string SortBy = "ItemCode",
    bool Descending = false
) : IRequest<GetAttentionItemsResponse>;