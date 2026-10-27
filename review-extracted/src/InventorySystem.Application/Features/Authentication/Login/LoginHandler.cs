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
    private readonly IAuditLogService _auditLogService;

    public LoginHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        IJwtService jwtService,
        IAuditLogService auditLogService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _auditLogService = auditLogService;
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
            await _auditLogService.LogAsync(
                action: "LoginFailed",
                entity: "User",
                details: $"Username: {request.Username}",
                status: "Failed",
                cancellationToken: cancellationToken);

            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        if (!user.IsActive)
        {
            await _auditLogService.LogAsync(
                action: "LoginFailed",
                entity: "User",
                entityId: user.Id.ToString(),
                details: $"Username: {user.Username}, Reason: Inactive account",
                status: "Failed",
                cancellationToken: cancellationToken);

            throw new UnauthorizedAccessException(
                "This user account is inactive.");
        }

        var passwordValid =
            _passwordService.VerifyPassword(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            await _auditLogService.LogAsync(
                action: "LoginFailed",
                entity: "User",
                entityId: user.Id.ToString(),
                details: $"Username: {user.Username}",
                status: "Failed",
                cancellationToken: cancellationToken);

            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        var roles =
            user.UserRoles
                .Select(x => x.Role.Name)
                .Distinct()
                .ToList();

        var rolePermissions =
            user.UserRoles
                .SelectMany(
                    x => x.Role.RolePermissions)
                .Select(
                    x => x.Permission.Code);

        var userPermissions =
            user.UserPermissions
                .Select(
                    x => x.Permission.Code);

        var permissions =
            rolePermissions
                .Concat(userPermissions)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

        var token =
            _jwtService.GenerateToken(
                user.Id,
                user.Username,
                roles,
                permissions);

        var expiresAt =
            DateTime.UtcNow.AddHours(8);

        await _auditLogService.LogAsync(
            action: "Login",
            entity: "User",
            entityId: user.Id.ToString(),
            details: $"Username: {user.Username}",
            cancellationToken: cancellationToken);

        return new LoginResponse(
            user.Id,
            user.Username,
            roles.FirstOrDefault() ?? "User",
            token,
            expiresAt);
    }
}