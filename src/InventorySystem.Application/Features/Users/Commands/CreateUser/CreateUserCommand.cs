using MediatR;

namespace InventorySystem.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Username,
    string Password,
    string Role,
    List<string> Permissions
) : IRequest<CreateUserResponse>;