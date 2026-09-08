using MediatR;

namespace InventorySystem.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(int Id) : IRequest<UserDetails?>;

public sealed record UserDetails(
    int Id,
    string Username,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IReadOnlyCollection<string> Permissions);