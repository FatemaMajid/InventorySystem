using FluentValidation;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Features.FinancialAnalysis.FinancialPeriods.Commands.ApproveFinancialPeriod;

public class ApproveFinancialPeriodCommandValidator
    : AbstractValidator<ApproveFinancialPeriodCommand>
{
    public ApproveFinancialPeriodCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(LocalizationKeys.FinancialPeriod.InvalidId);
    }
}
