using FluentValidation;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Features.Stores.Commands.CreateStore;

public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
    {
        // Store information
        RuleFor(x => x.Store.StoreCode)
            .NotEmpty()
            .WithMessage(LocalizationKeys.Store.CodeRequired)
            .MaximumLength(20)
            .WithMessage(LocalizationKeys.Store.CodeMaxLength);

        RuleFor(x => x.Store.StoreNameArabic)
            .NotEmpty()
            .WithMessage(LocalizationKeys.Store.ArabicNameRequired)
            .MaximumLength(100)
            .WithMessage(LocalizationKeys.Store.ArabicNameMaxLength);

        RuleFor(x => x.Store.StoreNameEnglish)
            .MaximumLength(100)
            .WithMessage(LocalizationKeys.Store.EnglishNameMaxLength);

        // Branch relationship
        RuleFor(x => x.Store.BranchCode)
            .NotEmpty()
            .WithMessage(LocalizationKeys.Store.BranchCodeRequired)
            .MaximumLength(20)
            .WithMessage(LocalizationKeys.Store.BranchCodeMaxLength);
    }
}