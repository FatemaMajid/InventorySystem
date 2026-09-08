using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities.Authorization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler
    : IRequestHandler<CreateRoleCommand, CreateRoleResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public CreateRoleCommandHandler(
        IApplicationDbContext context,
        IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<CreateRoleResponse> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Role name is required.");
        }

        var exists = await _context.Roles
            .AnyAsync(
                x => x.Name.ToLower() == name.ToLower(),
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
            throw new InvalidOperationException("One or more permissions were not found.");
        }

        var role = new Role
        {
            Name = name,
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim()
        };

        foreach (var permissionId in permissionIds)
        {
            role.RolePermissions.Add(
                new RolePermission
                {
                    PermissionId = permissionId
                });
        }

        _context.Roles.Add(role);

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(
            action: "Create",
            entity: "Role",
            entityId: role.Id.ToString(),
            details: $"Role: {role.Name}, Permissions: {permissionIds.Count}",
            cancellationToken: cancellationToken);

        return new CreateRoleResponse(
            role.Id,
            role.Name,
            role.Description,
            permissionIds);
    }
}