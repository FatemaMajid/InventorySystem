using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetAllFinancialPeriods;

public class GetAllFinancialPeriodsQueryHandler
    : IRequestHandler<GetAllFinancialPeriodsQuery, List<FinancialPeriodDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllFinancialPeriodsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<FinancialPeriodDto>> Handle(
        GetAllFinancialPeriodsQuery request,
        CancellationToken cancellationToken)
    {
        var periods =
            await _context.FinancialPeriods
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(
                    x => x.StartDate)
                .ThenByDescending(
                    x => x.EndDate)
                .ToListAsync(
                    cancellationToken);

        return _mapper.Map<
            List<FinancialPeriodDto>>(
                periods);
    }
}
