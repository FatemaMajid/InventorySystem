
using System.Text.Json;
using InventorySystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.Branches.Commands.UpdateBranch;

public class UpdateBranchCommandHandler
    : IRequestHandler<UpdateBranchCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public UpdateBranchCommandHandler(
        IApplicationDbContext context,
        IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(
        UpdateBranchCommand request,
        CancellationToken cancellationToken)
    {
        var branch = await _context.Branches
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (branch == null)
            return false;

        branch.BranchCode = request.Branch.BranchCode;
        branch.BranchNameArabic = request.Branch.BranchNameArabic;
        branch.BranchNameEnglish = request.Branch.BranchNameEnglish;
        branch.Address = request.Branch.Address;
        branch.Phone = request.Branch.Phone;
        branch.IsActive = request.Branch.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        var details = JsonSerializer.Serialize(new
        {
            branchNameArabic = branch.BranchNameArabic,
            branchNameEnglish = branch.BranchNameEnglish,
            branchCode = branch.BranchCode
        });

        await _auditLogService.LogAsync(
            action: "Update",
            entity: "Branch",
            entityId: branch.Id.ToString(),
            details: details,
            cancellationToken: cancellationToken);

        return true;
    }
}
