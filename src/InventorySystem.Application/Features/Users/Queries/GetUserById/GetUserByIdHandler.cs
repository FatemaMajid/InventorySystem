using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDetails?>
{
    private readonly IApplicationDbContext _context;

    public GetUserByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDetails?> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .Include(x => x.UserPermissions)
                .ThenInclude(x => x.Permission)
            .Where(x => x.Id == request.Id)
            .Select(x => new UserDetails(
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}