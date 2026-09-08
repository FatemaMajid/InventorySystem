using FluentValidation;

namespace InventorySystem.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .MaximumLength(200);

        RuleFor(x => x.Role)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Permissions)
            .NotNull();
    }
}