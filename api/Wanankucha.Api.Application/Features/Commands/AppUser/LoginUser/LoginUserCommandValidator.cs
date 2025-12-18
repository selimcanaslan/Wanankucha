using FluentValidation;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommandRequest>
{
    public LoginUserCommandValidator()
    {
        RuleFor(p => p.EmailOrUserName)
            .NotEmpty().WithMessage("Email or username can't be empty.");
        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password can't be empty.")
            .NotNull().WithMessage("Password can't be null.")
            .MinimumLength(6).WithMessage("Password must have at least 6 characters.");
    }
}