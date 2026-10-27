using InventorySystem.Application.DTOs.FinancialAnalysis.FinancialPeriod;
using MediatR;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.CreateFinancialPeriod;

public class CreateFinancialPeriodCommand : IRequest<int>
{
    public CreateFinancialPeriodDto FinancialPeriod { get; set; } = default!;
}
