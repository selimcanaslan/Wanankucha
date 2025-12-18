using FluentValidation;

namespace Wanankucha.Api.Application.Features.Commands.AppUser.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommandRequest>
{
    public CreateUserCommandValidator()
    {
        RuleFor(p => p.NameSurname)
            .NotEmpty().WithMessage("Fullname can't be empty.")
            .NotNull().WithMessage("Fullname can't be null.")
            .MaximumLength(50).WithMessage("Full name can't be longer than 50 characters.");

        RuleFor(p => p.UserName)
            .NotEmpty().WithMessage("Username can't be empty.")
            .MinimumLength(3).WithMessage("Username can't be smaller than 3 characters.");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Mail can't be empty.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password can't be empty.")
            .MinimumLength(6).WithMessage("Password can't be smaller than 6 characters.");

        RuleFor(p => p.PasswordConfirm)
            .NotEmpty().WithMessage("Confirm password can't be empty.")
            .Equal(p => p.Password).WithMessage("Passwords do not match.");
    }
}