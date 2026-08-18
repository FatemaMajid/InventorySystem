using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Authentication.Login;

public sealed class LoginHandler
    : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public LoginHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user =
            await _context.Users
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                        .ThenInclude(x => x.RolePermissions)
                            .ThenInclude(x => x.Permission)

                .Include(x => x.UserPermissions)
                    .ThenInclude(x => x.Permission)

                .FirstOrDefaultAsync(
                    x => x.Username == request.Username,
                    cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "This user account is inactive.");
        }

        var passwordValid =
            _passwordService.VerifyPassword(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }


        // ==========================================
        // Roles
        // ==========================================

        var roles =
            user.UserRoles
                .Select(x => x.Role.Name)
                .Distinct()
                .ToList();


        // ==========================================
        // Role Permissions
        // ==========================================

        var rolePermissions =
            user.UserRoles
                .SelectMany(
                    x => x.Role.RolePermissions)
                .Select(
                    x => x.Permission.Code);


        // ==========================================
        // User Permissions
        // ==========================================

        var userPermissions =
            user.UserPermissions
                .Select(
                    x => x.Permission.Code);


        // ==========================================
        // Combined Permissions
        // ==========================================

        var permissions =
            rolePermissions
                .Concat(userPermissions)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();


        // ==========================================
        // JWT
        // ==========================================

        var token =
            _jwtService.GenerateToken(
                user.Id,
                user.Username,
                roles,
                permissions);

        var expiresAt =
            DateTime.UtcNow.AddHours(8);


        return new LoginResponse(
            user.Id,
            user.Username,
            roles.FirstOrDefault() ?? "User",
            token,
            expiresAt);
    }
}