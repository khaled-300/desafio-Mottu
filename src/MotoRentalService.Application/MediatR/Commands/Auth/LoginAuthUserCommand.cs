using MediatR;
using MotoRentalService.Application.MediatR.Response.Auth;

namespace MotoRentalService.Application.MediatR.Commands.Auth
{
    public class LoginAuthUserCommand : IRequest<AuthCommandResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
