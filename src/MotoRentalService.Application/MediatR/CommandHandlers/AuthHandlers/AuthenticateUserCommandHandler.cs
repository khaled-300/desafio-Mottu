using AutoMapper;
using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Auth;
using MotoRentalService.Application.MediatR.Response.Auth;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.AuthHandlers
{
    public class AuthenticateUserCommandHandler : BaseCommandHandler<LoginAuthUserCommand, AuthCommandResult>
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IValidator<LoginAuthUserCommand> _validator;

        public AuthenticateUserCommandHandler(IUserAuthenticationService userAuthenticationService, IValidator<LoginAuthUserCommand> validator): base(validator)
        {
            _validator = validator; 
            _userAuthenticationService = userAuthenticationService;
        }
        public override async Task<AuthCommandResult> Handle(LoginAuthUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var token = await _userAuthenticationService.AuthenticateUserAsync(request.Email, request.Password, cancellationToken);
                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthCommandResult(success: false, message: "user is not valid");
                }
                return new AuthCommandResult(token, true, "User is authenticated successfully.");
            }
            catch (Exception ex)
            {
                return HandleException(ex); 
            }
        }
    }
}
