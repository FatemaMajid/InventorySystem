using MediatR;

namespace InventorySystem.Application.Features.Users.Queries.GetUserOptions;

public sealed record GetUserOptionsQuery : IRequest<UserOptionsResponse>;

public sealed record UserOptionsResponse(
    IReadOnlyCollection<UserRoleOption> Roles,
    IReadOnlyCollection<UserPermissionOption> Permissions);

public sealed record UserRoleOption(
    int Id,
    string Name,
    string? Description,
    IReadOnlyCollection<int> PermissionIds);

public sealed record UserPermissionOption(
    int Id,
    string Code,
    string Name);