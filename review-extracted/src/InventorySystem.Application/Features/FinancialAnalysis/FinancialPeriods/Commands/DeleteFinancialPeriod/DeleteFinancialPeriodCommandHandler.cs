using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.DeleteFinancialPeriod;

public class DeleteFinancialPeriodCommandHandler
    : IRequestHandler<
        DeleteFinancialPeriodCommand,
        bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;

    public DeleteFinancialPeriodCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
    }

    public async Task<bool> Handle(
        DeleteFinancialPeriodCommand request,
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
                "A financial period can only be deleted while it is in Draft status.");
        }

        period.IsDeleted =
            true;

        period.UpdatedAt =
            DateTime.UtcNow;

        period.UpdatedBy =
            _currentUser.Username;

        await _context.SaveChangesAsync(
            cancellationToken);

        await _auditLogService.LogAsync(
            action: "Delete",
            entity: "FinancialPeriod",
            entityId:
                period.Id.ToString(),
            details:
                $"StartDate: {period.StartDate:yyyy-MM-dd}, EndDate: {period.EndDate:yyyy-MM-dd}",
            cancellationToken:
                cancellationToken);

        return true;
    }
}
