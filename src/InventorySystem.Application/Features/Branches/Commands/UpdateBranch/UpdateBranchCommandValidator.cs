using FluentValidation;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Features.Branches.Commands.UpdateBranch;

public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        // Branch identity
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(LocalizationKeys.Branch.InvalidId);

        // Branch information
        RuleFor(x => x.Branch.BranchCode)
            .NotEmpty()
            .WithMessage(LocalizationKeys.Branch.CodeRequired)
            .MaximumLength(20)
            .WithMessage(LocalizationKeys.Branch.CodeMaxLength);

        RuleFor(x => x.Branch.BranchNameArabic)
            .NotEmpty()
            .WithMessage(LocalizationKeys.Branch.ArabicNameRequired)
            .MaximumLength(100)
            .WithMessage(LocalizationKeys.Branch.ArabicNameMaxLength);

        RuleFor(x => x.Branch.BranchNameEnglish)
            .MaximumLength(100)
            .WithMessage(LocalizationKeys.Branch.EnglishNameMaxLength);

        // Contact information
        RuleFor(x => x.Branch.Address)
            .MaximumLength(200)
            .WithMessage(LocalizationKeys.Branch.AddressMaxLength);

        RuleFor(x => x.Branch.Phone)
            .MaximumLength(20)
            .WithMessage(LocalizationKeys.Branch.PhoneMaxLength);
    }
}