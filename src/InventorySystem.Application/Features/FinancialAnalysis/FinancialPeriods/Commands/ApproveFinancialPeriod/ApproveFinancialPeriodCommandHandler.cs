using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetMissingFinancialFiles;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.ApproveFinancialPeriod;

public class ApproveFinancialPeriodCommandHandler : IRequestHandler<ApproveFinancialPeriodCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;
    private readonly IMediator _mediator;

    public ApproveFinancialPeriodCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
        _mediator = mediator;
    }

    public async Task<bool> Handle(
        ApproveFinancialPeriodCommand request,
        CancellationToken cancellationToken)
    {
        var period = await _context.FinancialPeriods
            .FirstOrDefaultAsync(
                x => x.Id == request.Id && !x.IsDeleted,
                cancellationToken);

        if (period is null)
            return false;

        if (period.Status == FinancialPeriodStatus.Approved)
        {
            throw new InvalidOperationException(
                LocalizationKeys.FinancialPeriod.AlreadyApproved);
        }

        if (period.Status != FinancialPeriodStatus.Ready)
        {
            throw new InvalidOperationException(
                LocalizationKeys.FinancialPeriod.FilesIncomplete);
        }

        var activeBranchCount = await _context.Branches
            .CountAsync(
                x => !x.IsDeleted &&
                     x.IsActive &&
                     x.BranchCode != "0" &&
                     x.BranchCode != "001" &&
                     x.BranchCode != "002",
                cancellationToken);

        var expectedFiles = 2 + (activeBranchCount * 4);

        var validUploads = await _context.FinancialUploads
            .CountAsync(
                x => x.FinancialPeriodId == period.Id &&
                     x.IsActive &&
                     !x.IsDeleted &&
                     x.Status == FinancialUploadStatus.Valid,
                cancellationToken);

        if (validUploads != expectedFiles)
        {
            throw new InvalidOperationException(
                LocalizationKeys.FinancialPeriod.FilesIncomplete);
        }

        var missingFiles = await _mediator.Send(
            new GetMissingFinancialFilesQuery(period.Id),
            cancellationToken);

        if (missingFiles is null || missingFiles.Count > 0)
        {
            throw new InvalidOperationException(
                LocalizationKeys.FinancialPeriod.FilesIncomplete);
        }

        period.Status = FinancialPeriodStatus.Approved;
        period.ApprovedAt = DateTime.UtcNow;
        period.ApprovedBy = _currentUser.Username;
        period.UpdatedAt = DateTime.UtcNow;
        period.UpdatedBy = _currentUser.Username;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(
                action: "Approve",
                entity: "FinancialPeriod",
                entityId: period.Id.ToString(),
                details: $"StartDate: {period.StartDate:yyyy-MM-dd}, EndDate: {period.EndDate:yyyy-MM-dd}, Files: {validUploads}/{expectedFiles}",
                cancellationToken: cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}
