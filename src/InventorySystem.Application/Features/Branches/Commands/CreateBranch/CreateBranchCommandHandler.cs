
using System.Text.Json;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.Entities;
using MediatR;

namespace InventorySystem.Application.Features.Branches.Commands.CreateBranch;

/// <summary>
/// Handles the CreateBranchCommand.
/// Responsible for creating a new branch in the database.
/// </summary>
public class CreateBranchCommandHandler
    : IRequestHandler<CreateBranchCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditLogService _auditLogService;

    public CreateBranchCommandHandler(
        IApplicationDbContext context,
        IAuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    public async Task<int> Handle(
        CreateBranchCommand request,
        CancellationToken cancellationToken)
    {
        var branch = new Branch
        {
            BranchCode = request.Branch.BranchCode,
            BranchNameArabic = request.Branch.BranchNameArabic,
            BranchNameEnglish = request.Branch.BranchNameEnglish,
            Address = request.Branch.Address,
            Phone = request.Branch.Phone,
            IsActive = request.Branch.IsActive
        };

        _context.Branches.Add(branch);

        await _context.SaveChangesAsync(cancellationToken);

        var details = JsonSerializer.Serialize(new
        {
            branchNameArabic = branch.BranchNameArabic,
            branchNameEnglish = branch.BranchNameEnglish,
            branchCode = branch.BranchCode
        });

        await _auditLogService.LogAsync(
            action: "Create",
            entity: "Branch",
            entityId: branch.Id.ToString(),
            details: details,
            cancellationToken: cancellationToken);

        return branch.Id;
    }
}
