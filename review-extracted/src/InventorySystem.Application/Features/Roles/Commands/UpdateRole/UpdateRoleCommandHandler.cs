using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities.Authorization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommandHandler
    : IRequestHandler<UpdateRoleCommand, UpdateRoleResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public UpdateRoleCommandHandler(
        IApplicationDbContext context,
        IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<UpdateRoleResponse> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await _context.Roles
            .Include(x => x.RolePermissions)
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (role is null)
        {
            throw new KeyNotFoundException("Role was not found.");
        }

        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Role name is required.");
        }

        var protectedRoleNames = new[]
        {
            "Manager",
            "Admin",
            "User"
        };

        var isProtectedRole =
            protectedRoleNames.Any(
                x => x.Equals(
                    role.Name,
                    StringComparison.OrdinalIgnoreCase));

        var changesProtectedName =
            !role.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase) &&
            protectedRoleNames.Any(
                x => x.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase));

        if (changesProtectedName)
        {
            throw new InvalidOperationException(
                $"Role name '{name}' is reserved and cannot be used.");
        }

        var exists = await _context.Roles
            .AnyAsync(
                x => x.Id != request.Id &&
                     x.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Role already exists.");
        }

        var permissionIds = request.PermissionIds
            .Distinct()
            .ToList();

        var validPermissionIds = await _context.Permissions
            .Where(x => permissionIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (validPermissionIds.Count != permissionIds.Count)
        {
            throw new InvalidOperationException(
                "One or more permissions were not found.");
        }

        role.Name = isProtectedRole
            ? role.Name
            : name;

        role.Description = string.IsNullOrWhiteSpace(request.Description)
            ? null
            : request.Description.Trim();

        role.RolePermissions.Clear();

        foreach (var permissionId in permissionIds)
        {
            role.RolePermissions.Add(
                new RolePermission
                {
                    PermissionId = permissionId
                });
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(
            action: "Update",
            entity: "Role",
            entityId: role.Id.ToString(),
            details: $"Role: {role.Name}, Permissions: {permissionIds.Count}",
            cancellationToken: cancellationToken);

        return new UpdateRoleResponse(
            role.Id,
            role.Name,
            role.Description,
            permissionIds);
    }
}