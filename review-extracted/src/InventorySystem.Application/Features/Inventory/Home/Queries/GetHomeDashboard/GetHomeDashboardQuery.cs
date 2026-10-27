using MediatR;

namespace InventorySystem.Application.Features.Inventory.Home.Queries.GetHomeDashboard;

public sealed record GetHomeDashboardQuery
    : IRequest<GetHomeDashboardResponse>;