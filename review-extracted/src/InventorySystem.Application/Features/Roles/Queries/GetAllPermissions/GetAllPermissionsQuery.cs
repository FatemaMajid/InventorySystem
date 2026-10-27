using MediatR;

namespace InventorySystem.Application.Features.Roles.Queries.GetAllPermissions;

public sealed record GetAllPermissionsQuery
    : IRequest<IReadOnlyCollection<PermissionListItem>>;

public sealed record PermissionListItem(
    int Id,
    string Code,
    string Name);