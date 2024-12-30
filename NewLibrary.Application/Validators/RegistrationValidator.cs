using FluentValidation;
using NewLibrary.Application.Commands.UserCommands;

namespace NewLibrary.Application.Validators
{
    public class RegistrationValidator : AbstractValidator<UserRegisterCommand>
    {
        public RegistrationValidator()
        {
            RuleFor(r => r.FullName).NotNull().NotEmpty().MinimumLength(3).MaximumLength(30);
            RuleFor(r => r.UserName).NotNull().NotEmpty().MaximumLength(30);
            RuleFor(r => r.Email).NotNull().NotEmpty().EmailAddress();
            RuleFor(r => r.Password).NotNull().NotEmpty().MinimumLength(6).MaximumLength(30);
            RuleFor(r => r.RePassword).NotNull().NotEmpty().MinimumLength(6).MaximumLength(30);
            RuleFor(r => r).Custom((r, context) =>
            {
                if (r.Password != r.RePassword)
                {
                    context.AddFailure("Password", "does not match");
                }
            });
        }
    }
}
