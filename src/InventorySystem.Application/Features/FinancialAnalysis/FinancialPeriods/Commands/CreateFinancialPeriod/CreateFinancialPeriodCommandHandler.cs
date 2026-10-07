using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.CreateFinancialPeriod;

public class CreateFinancialPeriodCommandHandler
    : IRequestHandler<CreateFinancialPeriodCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLogService;

    public CreateFinancialPeriodCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLogService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
    }

    public async Task<int> Handle(
        CreateFinancialPeriodCommand request,
        CancellationToken cancellationToken)
    {
        var startDate =
            request.FinancialPeriod.StartDate.Date;

        var endDate =
            request.FinancialPeriod.EndDate.Date;

        var period =
            new FinancialPeriod
            {
                StartDate = startDate,
                EndDate = endDate,
                Status =
                    FinancialPeriodStatus.Draft,
                CreatedAt =
                    DateTime.UtcNow,
                CreatedBy =
                    _currentUser.Username
            };

        _context.FinancialPeriods.Add(period);

        await _context.SaveChangesAsync(
            cancellationToken);

        await _auditLogService.LogAsync(
            action: "Create",
            entity: "FinancialPeriod",
            entityId:
                period.Id.ToString(),
            details:
                $"StartDate: {period.StartDate:yyyy-MM-dd}, EndDate: {period.EndDate:yyyy-MM-dd}",
            cancellationToken:
                cancellationToken);

        return period.Id;
    }
}
