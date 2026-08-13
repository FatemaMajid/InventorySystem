using FluentValidation;
using InventorySystem.Application.Common.Localization;

namespace InventorySystem.Application.Features.Stores.Commands.UpdateStore;

public class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
{
    public UpdateStoreCommandValidator()
    {
        // Store identity
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage(LocalizationKeys.Common.Invalid);

        // Store information
        RuleFor(x => x.Store.StoreCode)
            .NotEmpty()
            .WithMessage(LocalizationKeys.Store.Required)
            .MaximumLength(20)
            .WithMessage(LocalizationKeys.Common.Invalid);

        RuleFor(x => x.Store.StoreNameArabic)
            .NotEmpty()
            .WithMessage(LocalizationKeys.Store.Required)
            .MaximumLength(100)
            .WithMessage(LocalizationKeys.Common.Invalid);

        RuleFor(x => x.Store.StoreNameEnglish)
            .MaximumLength(100)
            .WithMessage(LocalizationKeys.Common.Invalid);

        // Branch relationship
        RuleFor(x => x.Store.BranchCode)
            .NotEmpty()
            .WithMessage(LocalizationKeys.Branch.Required)
            .MaximumLength(20)
            .WithMessage(LocalizationKeys.Common.Invalid);
    }
}