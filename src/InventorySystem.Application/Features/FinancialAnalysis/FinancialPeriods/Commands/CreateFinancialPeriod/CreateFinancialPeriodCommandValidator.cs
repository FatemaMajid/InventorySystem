using FluentValidation;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.CreateFinancialPeriod;

public class CreateFinancialPeriodCommandValidator
    : AbstractValidator<CreateFinancialPeriodCommand>
{
    public CreateFinancialPeriodCommandValidator()
    {
        RuleFor(x => x.FinancialPeriod.Year)
            .InclusiveBetween(2000, 2100)
            .WithMessage(LocalizationKeys.FinancialPeriod.InvalidYear);

        RuleFor(x => x.FinancialPeriod.Month)
            .InclusiveBetween(1, 12)
            .WithMessage(LocalizationKeys.FinancialPeriod.InvalidMonth);
    }
}
