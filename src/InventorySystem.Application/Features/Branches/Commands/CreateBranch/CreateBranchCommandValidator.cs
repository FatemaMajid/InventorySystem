using FluentValidation;

namespace InventorySystem.Application.Features.Branches.Commands.CreateBranch;

public class CreateBranchCommandValidator
    : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchCommandValidator()
    {
        RuleFor(x => x.Branch.BranchCode)
            .NotEmpty()
            .WithMessage("Branch code is required.")
            .MaximumLength(20)
            .WithMessage("Branch code must not exceed 20 characters.");

        RuleFor(x => x.Branch.BranchNameArabic)
            .NotEmpty()
            .WithMessage("Branch Arabic name is required.")
            .MaximumLength(100)
            .WithMessage("Branch Arabic name must not exceed 100 characters.");

        RuleFor(x => x.Branch.BranchNameEnglish)
            .MaximumLength(100)
            .WithMessage("Branch English name must not exceed 100 characters.");

        RuleFor(x => x.Branch.Address)
            .MaximumLength(200)
            .WithMessage("Address must not exceed 200 characters.");

        RuleFor(x => x.Branch.Phone)
            .MaximumLength(20)
            .WithMessage("Phone number must not exceed 20 characters.");
    }
}