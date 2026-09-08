using FluentValidation;

namespace InventorySystem.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(200);

        RuleFor(x => x.Role)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Permissions)
            .NotNull();
    }
}