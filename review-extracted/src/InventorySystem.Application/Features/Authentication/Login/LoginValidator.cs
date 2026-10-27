using FluentValidation;

namespace InventorySystem.Application.Features.Authentication.Login;

public sealed class LoginValidator
    : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(200);
    }
}