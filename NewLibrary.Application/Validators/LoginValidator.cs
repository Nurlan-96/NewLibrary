using FluentValidation;
using NewLibrary.Application.Commands.UserCommands;

namespace NewLibrary.Application.Validators
{
    public class LoginValidator : AbstractValidator<UserLoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(r => r.Username).NotNull().NotEmpty().MaximumLength(30);
            RuleFor(r => r.Password).NotNull().NotEmpty().MinimumLength(6).MaximumLength(30);
        }
    }
}
