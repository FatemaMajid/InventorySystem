
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetAllFinancialPeriods;

public class GetAllFinancialPeriodsQueryHandler : IRequestHandler<GetAllFinancialPeriodsQuery, List<FinancialPeriodDto>> {
    private readonly IApplicationDbContext _context;

    public GetAllFinancialPeriodsQueryHandler(IApplicationDbContext context){
        _context = context;
    }

    public async Task<List<FinancialPeriodDto>> Handle(GetAllFinancialPeriodsQuery request, CancellationToken cancellationToken){
        var periods = await _context.FinancialPeriods
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.StartDate)
            .ThenByDescending(x => x.EndDate)
            .ToListAsync(cancellationToken);

        return periods.Select(period => new FinancialPeriodDto
        {
            Id = period.Id,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            Status = period.Status,
            CreatedAt = period.CreatedAt,
            CreatedBy = period.CreatedBy,
            ApprovedAt = period.ApprovedAt,
            ApprovedBy = period.ApprovedBy
        }).ToList();
    }
}
