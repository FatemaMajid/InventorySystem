using MediatR;

namespace InventorySystem.Application.Features.InventorySessions.Queries.GetInventoryDashboard;

public record GetInventoryDashboardQuery(
    int SessionId
) : IRequest<GetInventoryDashboardResponse>;