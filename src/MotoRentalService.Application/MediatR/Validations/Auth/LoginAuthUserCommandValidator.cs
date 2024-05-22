using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Auth;

namespace MotoRentalService.Application.MediatR.Validations.Auth
{
    public class LoginAuthUserCommandValidator : AbstractValidator<LoginAuthUserCommand>
    {
        public LoginAuthUserCommandValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty().WithName("Email")
                .EmailAddress().WithName("Email");

            RuleFor(m => m.Password)
                .NotEmpty().WithName("Password")
                .MinimumLength(6).WithName("Password")
                .MaximumLength(50).WithName("Password");
        }
    }
}
