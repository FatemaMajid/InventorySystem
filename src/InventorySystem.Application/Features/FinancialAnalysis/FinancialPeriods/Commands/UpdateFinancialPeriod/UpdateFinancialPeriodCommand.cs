using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.UpdateFinancialPeriod;

public class UpdateFinancialPeriodCommand : IRequest<bool>
{
    public int Id { get; set; }

    public UpdateFinancialPeriodDto FinancialPeriod { get; set; } =
        default!;
}
