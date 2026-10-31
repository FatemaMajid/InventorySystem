
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodById;

public class GetFinancialPeriodByIdQueryHandler : IRequestHandler<GetFinancialPeriodByIdQuery, FinancialPeriodDto?> {
    private readonly IApplicationDbContext _context;

    public GetFinancialPeriodByIdQueryHandler(IApplicationDbContext context){
        _context = context;
    }

    public async Task<FinancialPeriodDto?> Handle(GetFinancialPeriodByIdQuery request, CancellationToken cancellationToken){
        var period = await _context.FinancialPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (period is null)
            return null;

        return new FinancialPeriodDto
        {
            Id = period.Id,
            StartDate = period.StartDate,
            EndDate = period.EndDate,
            Status = period.Status,
            CreatedAt = period.CreatedAt,
            CreatedBy = period.CreatedBy,
            ApprovedAt = period.ApprovedAt,
            ApprovedBy = period.ApprovedBy
        };
    }
}
