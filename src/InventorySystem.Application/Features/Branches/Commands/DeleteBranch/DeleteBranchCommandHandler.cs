using System.Text.Json;
using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Commands.DeleteBranch;

/// <summary>
/// Handles the DeleteBranchCommand.
/// </summary>
public class DeleteBranchCommandHandler
    : IRequestHandler<DeleteBranchCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public DeleteBranchCommandHandler(
        IApplicationDbContext context,
        IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(
        DeleteBranchCommand request,
        CancellationToken cancellationToken)
    {
        var branch = await _context.Branches
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (branch == null)
            return false;

        var details = JsonSerializer.Serialize(new
        {
            branchNameArabic = branch.BranchNameArabic,
            branchNameEnglish = branch.BranchNameEnglish,
            branchCode = branch.BranchCode
        });

        _context.Branches.Remove(branch);

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(
            action: "Delete",
            entity: "Branch",
            entityId: request.Id.ToString(),
            details: details,
            cancellationToken: cancellationToken);

        return true;
    }
}