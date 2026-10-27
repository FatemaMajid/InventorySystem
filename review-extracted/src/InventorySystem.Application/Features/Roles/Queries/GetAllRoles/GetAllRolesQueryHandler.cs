using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Roles.Queries.GetAllRoles;

public sealed class GetAllRolesQueryHandler
    : IRequestHandler<GetAllRolesQuery, IReadOnlyCollection<RoleListItem>>
{
    private readonly IApplicationDbContext _context;

    public GetAllRolesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<RoleListItem>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Roles
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new RoleListItem(
                x.Id,
                x.Name,
                x.Description,
                x.RolePermissions.Count,
                x.UserRoles.Count))
            .ToListAsync(cancellationToken);
    }
}