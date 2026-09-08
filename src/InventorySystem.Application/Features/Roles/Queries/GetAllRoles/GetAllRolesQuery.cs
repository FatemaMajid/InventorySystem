using MediatR;

namespace InventorySystem.Application.Features.Roles.Queries.GetAllRoles;

public sealed record GetAllRolesQuery : IRequest<IReadOnlyCollection<RoleListItem>>;

public sealed record RoleListItem(
    int Id,
    string Name,
    string? Description,
    int PermissionCount,
    int UserCount);