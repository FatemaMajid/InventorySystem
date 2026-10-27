using AutoMapper;
using InventorySystem.Application.Common.Interfaces;
using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodById;

public class GetFinancialPeriodByIdQueryHandler
    : IRequestHandler<GetFinancialPeriodByIdQuery, FinancialPeriodDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetFinancialPeriodByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FinancialPeriodDto?> Handle(
        GetFinancialPeriodByIdQuery request,
        CancellationToken cancellationToken)
    {
        var period = await _context.FinancialPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.Id == request.Id &&
                    !x.IsDeleted,
                cancellationToken);

        return period is null
            ? null
            : _mapper.Map<FinancialPeriodDto>(period);
    }
}
