using FluentValidation;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Features.Branches.Commands.CreateBranch;

public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
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