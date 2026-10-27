using MediatR;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryComparisonExport;

public record GetInventoryComparisonExportQuery(
    int SessionId,
    string? ItemCode = null,
    string? ItemName = null,
    int? CategoryId = null,
    int? UnitId = null,
    string? Status = null,
    string SortBy = "ItemCode",
    bool Descending = false
) : IRequest<IReadOnlyList<InventoryComparisonExportItem>>;