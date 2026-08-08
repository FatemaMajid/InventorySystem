using FluentValidation;

namespace InventorySystem.Application.Features.Stores.Commands.CreateStore;

/// <summary>
/// Validates store creation requests.
/// </summary>
public class CreateStoreCommandValidator
    : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
    {
        RuleFor(x => x.Store.StoreCode)
            .NotEmpty()
            .WithMessage("Store code is required.")
            .MaximumLength(20)
            .WithMessage("Store code must not exceed 20 characters.");

        RuleFor(x => x.Store.StoreNameArabic)
            .NotEmpty()
            .WithMessage("Store Arabic name is required.")
            .MaximumLength(100)
            .WithMessage("Store Arabic name must not exceed 100 characters.");

        RuleFor(x => x.Store.StoreNameEnglish)
            .MaximumLength(100)
            .WithMessage("Store English name must not exceed 100 characters.");

        RuleFor(x => x.Store.BranchCode)
            .NotEmpty()
            .WithMessage("Branch code is required.")
            .MaximumLength(20)
            .WithMessage("Branch code must not exceed 20 characters.");
    }
}