using FluentValidation;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.CreateFinancialPeriod;

public class CreateFinancialPeriodCommandValidator
    : AbstractValidator<CreateFinancialPeriodCommand>
{
    public CreateFinancialPeriodCommandValidator()
    {
        RuleFor(x => x.FinancialPeriod.StartDate)
            .NotEmpty()
            .WithMessage(
                "Start date is required.");

        RuleFor(x => x.FinancialPeriod.EndDate)
            .NotEmpty()
            .WithMessage(
                "End date is required.");

        RuleFor(x => x.FinancialPeriod.EndDate)
            .GreaterThanOrEqualTo(
                x => x.FinancialPeriod.StartDate)
            .WithMessage(
                "End date must be greater than or equal to start date.");
    }
}
