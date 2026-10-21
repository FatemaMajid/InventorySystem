using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.DeleteFinancialPeriod;

public class DeleteFinancialPeriodCommand
    : IRequest<bool>
{
    public int Id { get; set; }
}
