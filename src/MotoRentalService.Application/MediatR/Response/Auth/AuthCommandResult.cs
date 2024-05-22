using MotoRentalService.Application.MediatR.Response.Common;
using MotoRentalService.Application.MediatR.Response.Rental;

namespace MotoRentalService.Application.MediatR.Response.Auth
{
    /// <summary>
    /// Represents the authentication result containing a token.
    /// </summary>
    public class AuthCommandResult : ApiResult
    {
        /// <summary>
        /// Token issued after a successful authentication.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthCommandResult"/> class.
        /// </summary>
        public AuthCommandResult()
        {
        }


        public AuthCommandResult(string token = null, bool success = false, string message = null)
        {
            Token = token;
            Success = success;
            Message = message;
        }
    }
}
