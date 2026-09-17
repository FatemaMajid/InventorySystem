using MediatR;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Application.Common.Interfaces;

namespace InventorySystem.Application.Features.Users.Commands.DeleteUser;

public sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;

    public DeleteUserHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
    }

    public async Task<Unit> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsManager)
        {
            throw new UnauthorizedAccessException(
                "Only Manager can delete users.");
        }

        if (_currentUser.UserId == request.Id)
        {
            throw new InvalidOperationException(
                "You cannot delete your own account.");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                "User was not found.");
        }

        var username = user.Username;

        _context.Users.Remove(user);

        await _context.SaveChangesAsync(
            cancellationToken);

        await _auditLogService.LogAsync(
            action: "Delete",
            entity: "User",
            entityId: request.Id.ToString(),
            details: $"Username: {username}",
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}