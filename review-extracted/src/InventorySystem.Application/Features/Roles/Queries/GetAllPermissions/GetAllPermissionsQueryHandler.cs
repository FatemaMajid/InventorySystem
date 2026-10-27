using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Roles.Queries.GetAllPermissions;

public sealed class GetAllPermissionsQueryHandler
    : IRequestHandler<GetAllPermissionsQuery, IReadOnlyCollection<PermissionListItem>>
{
    private readonly IApplicationDbContext _context;

    public GetAllPermissionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<PermissionListItem>> Handle(
        GetAllPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new PermissionListItem(
                x.Id,
                x.Code,
                x.Name))
            .ToListAsync(cancellationToken);
    }
}