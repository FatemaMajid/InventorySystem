using MediatR;

namespace InventorySystem.Application.Features.Inventory.InventorySessions.Queries.GetInventoryDashboard;

public record GetInventoryDashboardQuery(
    int SessionId
) : IRequest<GetInventoryDashboardResponse>;