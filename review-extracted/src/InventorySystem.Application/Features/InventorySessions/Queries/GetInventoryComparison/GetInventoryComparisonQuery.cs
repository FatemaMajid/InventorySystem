using MediatR;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparison;

public record GetInventoryComparisonQuery(
    int SessionId,
    string? ItemCode = null,
    string? ItemName = null,
    int? CategoryId = null,
    int? UnitId = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 20,
    string SortBy = "ItemCode",
    bool Descending = false
) : IRequest<GetInventoryComparisonResponse>;