using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Queries.GetFinancialPeriodById;

public class GetFinancialPeriodByIdQuery
    : IRequest<FinancialPeriodDto?>
{
    public int Id { get; set; }
}
