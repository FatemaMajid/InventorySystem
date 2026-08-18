namespace InventorySystem.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserResponse(
    int Id,
    string Username,
    string Role,
    bool IsActive,
    IReadOnlyCollection<string> Permissions
);