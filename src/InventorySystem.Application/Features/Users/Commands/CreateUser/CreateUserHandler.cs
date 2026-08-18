using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities.Authorization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserHandler
    : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentUserService _currentUser;

    public CreateUserHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        ICurrentUserService currentUser)
    {
        _context = context;
        _passwordService = passwordService;
        _currentUser = currentUser;
    }

    public async Task<CreateUserResponse> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Only Manager and Admin can create users
        if (!_currentUser.IsManager && !_currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to create users.");
        }

        var username = request.Username.Trim();
        var roleName = request.Role.Trim();

        // Username must be unique
        var usernameExists =
            await _context.Users.AnyAsync(
                x => x.Username == username,
                cancellationToken);

        if (usernameExists)
        {
            throw new InvalidOperationException(
                "Username already exists.");
        }

        // Find requested role
        var role =
            await _context.Roles.FirstOrDefaultAsync(
                x => x.Name == roleName,
                cancellationToken);

        if (role is null)
        {
            throw new InvalidOperationException(
                $"Role '{roleName}' was not found.");
        }

        // Admin cannot create Manager
        if (_currentUser.IsAdmin &&
            role.Name.Equals(
                "Manager",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException(
                "Admin cannot create a Manager.");
        }

        // Get requested permissions
        var requestedPermissions =
            request.Permissions
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

        var permissions =
            await _context.Permissions
                .Where(x =>
                    requestedPermissions.Contains(x.Code))
                .ToListAsync(cancellationToken);

        if (permissions.Count != requestedPermissions.Count)
        {
            var found =
                permissions
                    .Select(x => x.Code)
                    .ToHashSet(
                        StringComparer.OrdinalIgnoreCase);

            var missing =
                requestedPermissions
                    .Where(x => !found.Contains(x))
                    .ToList();

            throw new InvalidOperationException(
                $"Invalid permissions: {string.Join(", ", missing)}");
        }

        // Create user
        var user = new User
        {
            Username = username,
            PasswordHash =
                _passwordService.HashPassword(
                    request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(
            cancellationToken);

        // Assign role
        _context.UserRoles.Add(
            new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });

        // Assign individual permissions
        foreach (var permission in permissions)
        {
            _context.UserPermissions.Add(
                new UserPermission
                {
                    UserId = user.Id,
                    PermissionId = permission.Id
                });
        }

        await _context.SaveChangesAsync(
            cancellationToken);

        return new CreateUserResponse(
            user.Id,
            user.Username,
            role.Name,
            user.IsActive,
            permissions
                .Select(x => x.Code)
                .ToList());
    }
}