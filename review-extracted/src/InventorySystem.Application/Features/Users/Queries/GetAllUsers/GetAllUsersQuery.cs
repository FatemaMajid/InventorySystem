using MediatR;

namespace InventorySystem.Application.Features.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<IReadOnlyCollection<UserListItem>>;

public sealed record UserListItem(
    int Id,
    string Username,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyCollection<string> Permissions);