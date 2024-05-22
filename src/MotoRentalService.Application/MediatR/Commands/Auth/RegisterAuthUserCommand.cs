using MediatR;
using MotoRentalService.Application.MediatR.Response.Auth;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.MediatR.Commands.Auth
{
    public class RegisterAuthUserCommand : IRequest<AuthCommandResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
