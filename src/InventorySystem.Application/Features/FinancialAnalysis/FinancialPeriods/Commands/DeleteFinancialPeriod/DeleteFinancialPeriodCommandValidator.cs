using FluentValidation;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.DeleteFinancialPeriod;

public class DeleteFinancialPeriodCommandValidator
    : AbstractValidator<DeleteFinancialPeriodCommand>
{
    public DeleteFinancialPeriodCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}
