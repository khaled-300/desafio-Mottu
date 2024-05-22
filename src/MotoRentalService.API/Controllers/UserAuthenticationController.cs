using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRentalService.Application.MediatR.Commands.Auth;
using MotoRentalService.Application.MediatR.Response.Auth;
using MotoRentalService.Application.Request.Auth;

namespace MotoRentalService.API.Controllers
{
    /// <summary>
    /// Controller Handles authentication requests such as registration and login for users.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserAuthenticationController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILogger<UserAuthenticationController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticationController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator to handle command sending.</param>
        /// <param name="logger">Logger for logging any relevant information or errors.</param>
        public UserAuthenticationController(IMediator mediator, ILogger<UserAuthenticationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }


        /// <summary>
        /// Registers a new user with authentication credentials and role.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /register
        ///     {
        ///        "email": "user@example.com",
        ///        "password": "your_password",
        ///        "role": "Admin"
        ///     }
        /// </remarks>
        /// <param name="request">The request containing the email, password, and role for the new user.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A newly created user authentication details.</returns>
        [HttpPost("register")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AuthCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateAuthUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new RegisterAuthUserCommand
                {
                    Email = request.Email,
                    Password = request.Password,
                    Role = request.Role
                };
                
                var result = await _mediator.Send(command, cancellationToken);
                if (result.Success)
                    return CreatedAtAction(nameof(AuthenticateUser), result);

                _logger.LogWarning("Failed to register new user: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


        /// <summary>
        /// Authenticates a user based on email and password.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /login
        ///     {
        ///        "email": "user@example.com",
        ///        "password": "your_password"
        ///     }
        /// </remarks>
        /// <param name="request">The login request containing the user's email and password.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>An authentication result indicating the success or failure of the login attempt.</returns>
        [HttpPost("login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AuthCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AuthenticateUser([FromBody] LoginAuthUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new LoginAuthUserCommand
                {
                    Email = request.Email,
                    Password = request.Password
                };
                var result = await _mediator.Send(command, cancellationToken);
                if (result.Success)
                    return Ok(result);

                _logger.LogWarning("Authentication failed: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user authentication.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
