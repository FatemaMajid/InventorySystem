using MediatR;

namespace InventorySystem.Application.Features.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(int Id) : IRequest<RoleDetailsResponse>;

public sealed record RoleDetailsResponse(
    int Id,
    string Name,
    string? Description,
    IReadOnlyCollection<int> PermissionIds);