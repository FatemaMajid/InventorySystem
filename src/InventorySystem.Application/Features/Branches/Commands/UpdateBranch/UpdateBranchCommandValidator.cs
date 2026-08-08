// ============================================================
// Project      : Inventory System
// Layer        : Application
// Feature      : Branches
// File         : UpdateBranchCommandValidator.cs
// Description  : Validates branch update requests.
// Author       : Fatema Majid
// ============================================================

using FluentValidation;

namespace InventorySystem.Application.Features.Branches.Commands.UpdateBranch;

/// <summary>
/// Validates the UpdateBranchCommand.
/// </summary>
public class UpdateBranchCommandValidator
    : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Branch ID must be greater than zero.");

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