using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRentalService.API.Models;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.User;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.Response.DeliveryUser;
using MotoRentalService.Application.Request.DeliveryUser;
using MotoRentalService.Domain.ValueObjects;
using System.Security.Claims;

namespace MotoRentalService.API.Controllers
{
    /// <summary>
    /// Controller for managing users.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructor for UserController.
        /// </summary>
        /// <param name="mediator">The Mediator instance.</param>
        /// <param name="logger">The Logger instance.</param>
        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a users by ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An ActionResult containing the user details.</returns>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeliveryUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserId(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new GetUserByIdQuery { Id = id }, cancellationToken);
                if (result == null)
                {
                    _logger.LogWarning("user not found with ID: {Id}", id);
                    return NotFound($"No user found with ID: {id}");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching user details with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="request">The request to create a user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An ActionResult indicating the result of the operation.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeliveryUserCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DeliveryUserCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromForm] CreateDeliveryUserRequest request, CancellationToken cancellationToken)
        {
            // the there issue with .net 8 that when i set the name of the method with ending async it canot find the action which odd
            // and the error i get is System.InvalidOperationException: No route matches the supplied values. so the name of the method is not with the naming convention.
            try
            {
                var user = GetUserDataFromToken();
                var command = new CreateUserDeliveryCommand
                {
                    AuthUserId = user.Id,
                    CNPJ = request.CNPJ,
                    DateOfBirth = request.DateOfBirth,
                    LicenseImage = request.LicenseImage,
                    LicenseNumber = request.LicenseNumber,
                    LicenseType = request.LicenseType,
                    Name = request.Name
                };

                var result = await _mediator.Send(command, cancellationToken);
                if (result.Success)
                    return CreatedAtAction(nameof(GetUserId), new { id = result.DeliveryUser?.Id }, result);

                _logger.LogWarning("Failed to create user: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="request">The request to update a user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An ActionResult indicating the result of the operation.</returns>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeliveryUserCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DeliveryUserCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromForm] UpdateDeliveryUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new UpdateDeliveryUserCommand
                {
                    Id = id,
                    LicenseImage = request.LicenseImage,
                    LicenseNumber = request.LicenseNumber,
                    LicenseType = request.LicenseType,
                };

                var user = GetUserDataFromToken();
                if (user != null && !user.Id.Equals(command.Id))
                {
                    return Forbid("You are not authorized to update this user.");
                }

                var result = await _mediator.Send(command, cancellationToken);
                if (result.Success)
                    return Ok(result);

                _logger.LogWarning("Failed to update user: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes a user by Id.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An ActionResult indicating the result of the operation.</returns>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeliveryUserCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DeliveryUserCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new DeleteDeliveryUserCommand { Id = id }, cancellationToken);
                if (result.Success)
                    return Ok(result);

                _logger.LogWarning("Failed to delete user: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        private AuthenticatedUserModel GetUserDataFromToken()
        {
            var authUser = HttpContext.User.Claims;
            var user = new AuthenticatedUserModel
            {
                Id = Convert.ToInt32(authUser.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)?.Value),
                Email = authUser.FirstOrDefault(_ => _.Type == ClaimTypes.Email)?.Value,
                Role = Enum.Parse<UserRole>(authUser.FirstOrDefault(_ => _.Type == ClaimTypes.Role).Value)
            };
            return user;
        }
    }
}
