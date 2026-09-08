using MediatR;

namespace InventorySystem.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    int Id,
    string Username,
    string? Password,
    string Role,
    List<string> Permissions,
    bool IsActive) : IRequest<UpdateUserResponse>;

public sealed record UpdateUserResponse(
    int Id,
    string Username,
    string Role,
    bool IsActive,
    IReadOnlyCollection<string> Permissions);