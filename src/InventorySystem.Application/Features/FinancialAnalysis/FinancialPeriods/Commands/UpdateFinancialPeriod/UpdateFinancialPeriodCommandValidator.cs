using FluentValidation;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.UpdateFinancialPeriod;

public class UpdateFinancialPeriodCommandValidator
    : AbstractValidator<UpdateFinancialPeriodCommand>
{
    public UpdateFinancialPeriodCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.FinancialPeriod.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.FinancialPeriod.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.");

        RuleFor(x => x.FinancialPeriod.EndDate)
            .GreaterThanOrEqualTo(
                x => x.FinancialPeriod.StartDate)
            .WithMessage(
                "End date must be greater than or equal to start date.");
    }
}
