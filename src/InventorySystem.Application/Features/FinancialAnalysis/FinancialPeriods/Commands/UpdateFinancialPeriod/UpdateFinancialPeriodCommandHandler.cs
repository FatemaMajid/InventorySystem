using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.UpdateFinancialPeriod;

public class UpdateFinancialPeriodCommandHandler
    : IRequestHandler<
        UpdateFinancialPeriodCommand,
        bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;

    public UpdateFinancialPeriodCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(
        UpdateFinancialPeriodCommand request,
        CancellationToken cancellationToken)
    {
        var period =
            await _context.FinancialPeriods
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.Id &&
                        !x.IsDeleted,
                    cancellationToken);

        if (period is null)
            return false;

        if (period.Status !=
            FinancialPeriodStatus.Draft)
        {
            throw new InvalidOperationException(
                "A financial period can only be updated while it is in Draft status.");
        }

        var oldStartDate =
            period.StartDate;

        var oldEndDate =
            period.EndDate;

        period.StartDate =
            request.FinancialPeriod
                .StartDate
                .Date;

        period.EndDate =
            request.FinancialPeriod
                .EndDate
                .Date;

        period.UpdatedAt =
            DateTime.UtcNow;

        period.UpdatedBy =
            _currentUser.Username;

        await _context.SaveChangesAsync(
            cancellationToken);

        await _auditLogService.LogAsync(
            action: "Update",
            entity: "FinancialPeriod",
            entityId: period.Id.ToString(),
            details:
                $"StartDate: {oldStartDate:yyyy-MM-dd} -> {period.StartDate:yyyy-MM-dd}, " +
                $"EndDate: {oldEndDate:yyyy-MM-dd} -> {period.EndDate:yyyy-MM-dd}",
            cancellationToken:
                cancellationToken);

        return true;
    }
}
