using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.Common.Localization;
using InventorySystem.Domain.FinancialAnalysis.Entities;
using InventorySystem.Domain.FinancialAnalysis.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        var year = request.FinancialPeriod.Year;
        var month = request.FinancialPeriod.Month;

        var exists = await _context.FinancialPeriods
            .AnyAsync(
                x =>
                    !x.IsDeleted &&
                    x.Year == year &&
                    x.Month == month,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                LocalizationKeys.FinancialPeriod.AlreadyExists);
        }

        var period = new FinancialPeriod
        {
            Year = year,
            Month = month,
            Status = FinancialPeriodStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.Username
        };

        _context.FinancialPeriods.Add(period);

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLogService.LogAsync(
            action: "Create",
            entity: "FinancialPeriod",
            entityId: period.Id.ToString(),
            details: $"Year: {period.Year}, Month: {period.Month}",
            cancellationToken: cancellationToken);

        return period.Id;
    }
}
