using MediatR;

namespace InventorySystem.Application.Features.Roles.Commands.UpdateRole;

public sealed record UpdateRoleCommand(
    int Id,
    string Name,
    string? Description,
    List<int> PermissionIds) : IRequest<UpdateRoleResponse>;

public sealed record UpdateRoleResponse(
    int Id,
    string Name,
    string? Description,
    IReadOnlyCollection<int> PermissionIds);