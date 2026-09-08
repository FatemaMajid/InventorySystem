using MediatR;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities.Authorization;

namespace InventorySystem.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;

    public UpdateUserHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService)
    {
        _context = context;
        _passwordService = passwordService;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
    }

    public async Task<UpdateUserResponse> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(x => x.UserRoles)
            .Include(x => x.UserPermissions)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException("User was not found.");
        }

        if (_currentUser.UserId == user.Id && !request.IsActive)
        {
            throw new InvalidOperationException(
                "You cannot deactivate your own account.");
        }

        var username = request.Username.Trim();
        var roleName = request.Role.Trim();

        var usernameExists = await _context.Users
            .AnyAsync(
                x => x.Id != user.Id && x.Username == username,
                cancellationToken);

        if (usernameExists)
        {
            throw new InvalidOperationException(
                "Username already exists.");
        }

        var role = await _context.Roles
            .FirstOrDefaultAsync(
                x => x.Name == roleName,
                cancellationToken);

        if (role is null)
        {
            throw new InvalidOperationException(
                $"Role '{roleName}' was not found.");
        }

        if (_currentUser.IsAdmin &&
            role.Name.Equals(
                "Manager",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                "Admin cannot assign the Manager role.");
        }

        var requestedPermissions = request.Permissions
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var permissions = await _context.Permissions
            .Where(x => requestedPermissions.Contains(x.Code))
            .ToListAsync(cancellationToken);

        if (permissions.Count != requestedPermissions.Count)
        {
            var found = permissions
                .Select(x => x.Code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var missing = requestedPermissions
                .Where(x => !found.Contains(x))
                .ToList();

            throw new InvalidOperationException(
                $"Invalid permissions: {string.Join(", ", missing)}");
        }

        user.Username = username;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash =
                _passwordService.HashPassword(request.Password);
        }

        _context.UserRoles.RemoveRange(user.UserRoles);
        _context.UserPermissions.RemoveRange(user.UserPermissions);

        user.UserRoles = new List<UserRole>
        {
            new()
            {
                UserId = user.Id,
                RoleId = role.Id
            }
        };

        user.UserPermissions = permissions
            .Select(permission => new UserPermission
            {
                UserId = user.Id,
                PermissionId = permission.Id
            })
            .ToList();

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(
            action: "Update",
            entity: "User",
            entityId: user.Id.ToString(),
            details: $"Username: {user.Username}, Role: {role.Name}, Active: {user.IsActive}",
            cancellationToken: cancellationToken);

        return new UpdateUserResponse(
            user.Id,
            user.Username,
            role.Name,
            user.IsActive,
            permissions.Select(x => x.Code).ToList());
    }
}