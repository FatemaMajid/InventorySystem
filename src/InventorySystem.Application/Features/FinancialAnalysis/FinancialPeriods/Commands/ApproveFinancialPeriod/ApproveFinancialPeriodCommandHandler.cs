using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.ApproveFinancialPeriod;

public class ApproveFinancialPeriodCommandHandler
    : IRequestHandler<ApproveFinancialPeriodCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;

    public ApproveFinancialPeriodCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(
        ApproveFinancialPeriodCommand request,
        CancellationToken cancellationToken)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.Id &&
                    !x.IsDeleted,
                cancellationToken);

        if (period is null)
            return false;

        if (period.Status == FinancialPeriodStatus.Approved)
        {
            throw new InvalidOperationException(
                LocalizationKeys.FinancialPeriod.AlreadyApproved);
        }

        var activeBranchCount = await _context.Branches.CountAsync(
            x =>
                !x.IsDeleted &&
                x.IsActive &&
                x.BranchCode != "0" &&
                x.BranchCode != "001" &&
                x.BranchCode != "002",
            cancellationToken);

        var expectedFiles =
            2 +
            (activeBranchCount * 2) +
            activeBranchCount +
            activeBranchCount;

        var validUploads = await _context.FinancialUploads
            .CountAsync(
                x =>
                    x.FinancialPeriodId == period.Id &&
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.Status == FinancialUploadStatus.Valid,
                cancellationToken);

        if (validUploads != expectedFiles)
        {
            throw new InvalidOperationException(
                LocalizationKeys.FinancialPeriod.FilesIncomplete);
        }

        period.Status = FinancialPeriodStatus.Approved;
        period.ApprovedAt = DateTime.UtcNow;
        period.ApprovedBy = _currentUser.Username;
        period.UpdatedAt = DateTime.UtcNow;
        period.UpdatedBy = _currentUser.Username;

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(
            action: "Approve",
            entity: "FinancialPeriod",
            entityId: period.Id.ToString(),
            details:
                $"Year: {period.Year}, Month: {period.Month}, Files: {validUploads}/{expectedFiles}",
            cancellationToken: cancellationToken);

        return true;
    }
}
