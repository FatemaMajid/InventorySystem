using MediatR;

namespace InventorySystem.Application.Features.Home.Queries.GetHomeDashboard;

public sealed record GetHomeDashboardQuery
    : IRequest<GetHomeDashboardResponse>;