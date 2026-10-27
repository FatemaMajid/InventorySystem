using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Users.Queries.GetUserOptions;

public sealed class GetUserOptionsHandler : IRequestHandler<GetUserOptionsQuery, UserOptionsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetUserOptionsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserOptionsResponse> Handle(
        GetUserOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new UserRoleOption(
                x.Id,
                x.Name,
                x.Description,
                x.RolePermissions
                    .Select(rolePermission => rolePermission.PermissionId)
                    .ToList()))
            .ToListAsync(cancellationToken);

        var permissions = await _context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new UserPermissionOption(
                x.Id,
                x.Code,
                x.Name))
            .ToListAsync(cancellationToken);

        return new UserOptionsResponse(
            roles,
            permissions);
    }
}