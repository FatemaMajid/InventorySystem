using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.ApproveFinancialPeriod;

public class ApproveFinancialPeriodCommand : IRequest<bool>
{
    public int Id { get; set; }
}
