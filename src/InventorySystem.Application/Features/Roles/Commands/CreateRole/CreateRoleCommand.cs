using MediatR;

namespace InventorySystem.Application.Features.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string? Description,
    List<int> PermissionIds) : IRequest<CreateRoleResponse>;

public sealed record CreateRoleResponse(
    int Id,
    string Name,
    string? Description,
    IReadOnlyCollection<int> PermissionIds);