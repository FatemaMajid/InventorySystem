using InventorySystem.Domain.Enums;
using MediatR;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventorySessions;

public record GetInventorySessionsQuery(
    int? BranchId = null,
    int? StoreId = null,
    InventoryType? InventoryType = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    string? Status = null,
    string? SessionNumber = null,
    int PageNumber = 1,
    int PageSize = 20,
    string SortBy = "InventoryDate",
    bool Descending = true
) : IRequest<GetInventorySessionsResponse>;