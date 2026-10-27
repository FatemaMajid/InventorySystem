using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Users.Queries.GetAllUsers;

public sealed class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IReadOnlyCollection<UserListItem>>
{
    private readonly IApplicationDbContext _context;

    public GetAllUsersHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UserListItem>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .Include(x => x.UserPermissions)
                .ThenInclude(x => x.Permission)
            .OrderBy(x => x.Username)
            .Select(x => new UserListItem(
                x.Id,
                x.Username,
                x.UserRoles
                    .Select(r => r.Role.Name)
                    .FirstOrDefault() ?? "User",
                x.IsActive,
                x.CreatedAt,
                x.UpdatedAt,
                x.UserPermissions
                    .Select(p => p.Permission.Code)
                    .ToList()))
            .ToListAsync(cancellationToken);
    }
}