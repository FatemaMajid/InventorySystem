using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler
    : IRequestHandler<GetRoleByIdQuery, RoleDetailsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetRoleByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoleDetailsResponse> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new RoleDetailsResponse(
                x.Id,
                x.Name,
                x.Description,
                x.RolePermissions
                    .Select(rolePermission => rolePermission.PermissionId)
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (role is null)
            return null!;

        return role;
    }
}