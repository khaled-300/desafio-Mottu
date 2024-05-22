using AutoMapper;
using FluentValidation;
using MediatR;
using MotoRentalService.Application.MediatR.Commands.Auth;
using MotoRentalService.Application.MediatR.Response.Auth;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.AuthHandlers
{
    public class RegisterUserCommandHandler : BaseCommandHandler<RegisterAuthUserCommand, AuthCommandResult>
    {
        private readonly IUserAuthenticationService _userAuthenticationService;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterAuthUserCommand> _validator;

        public RegisterUserCommandHandler(IMapper mapper, IUserAuthenticationService userAuthenticationService, IValidator<RegisterAuthUserCommand> validator): base(validator)
        {
            _validator = validator;
            _userAuthenticationService = userAuthenticationService;
            _mapper = mapper;
        }

        public override async Task<AuthCommandResult> Handle(RegisterAuthUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var result = new AuthCommandResult();
                var user = _mapper.Map<Users>(request);
                var token = await _userAuthenticationService.RegisterUserAsync(user, cancellationToken);
                if (string.IsNullOrWhiteSpace(token))
                {
                    result.AddError("User is not valid");
                    return result;
                }

                result.SetSuccess("User has been created successfully.");
                result.Token = token;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
