using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.Response.Plans;
using MotoRentalService.Application.Request.Plans;

namespace MotoRentalService.API.Controllers
{
    /// <summary>
    /// Controller for managing rental plans.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlansController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PlansController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlansController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator to handle request/response interactions.</param>
        /// <param name="logger">Logger for logging information and errors.</param>
        public PlansController(IMediator mediator, ILogger<PlansController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a specific rental plan by ID.
        /// </summary>
        /// <param name="id">The ID of the rental plan to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token for canceling the request.</param>
        /// <returns>An IActionResult that contains the fetched rental plan or an error message.</returns>
        [HttpGet("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlanDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPlanById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new GetPlanByIdQuery { Id = id }, cancellationToken);
                if (result == null)
                {
                    _logger.LogWarning("Rental plan not found with ID: {Id}", id);
                    return NotFound($"No rental plan found with ID: {id}");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching rental plan details with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves all rental plans.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for canceling the request.</param>
        /// <returns>An IActionResult that contains the fetched all rental plans or an error message.</returns>
        [HttpGet("get-all")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<PlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPlansAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new GetPlansQuery(), cancellationToken);
                if (!result.Any())
                {
                    _logger.LogWarning("No rental plans was not found. result: {0}", result.Count());
                    return NotFound("No rental plans were found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching rental plans details");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Creates a new rental plan.
        /// </summary>
        /// <param name="request">The request with the details needed to create a plan.</param>
        /// <param name="cancellationToken">Cancellation token for canceling the request.</param>
        /// <returns>An IActionResult that indicates the result of creating the plan.</returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlansCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PlansCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequest request, CancellationToken cancellationToken)
        {
            // the there issue with .net 8 that when i set the name of the method with ending async it canot find the action which odd
            // and the error i get is System.InvalidOperationException: No route matches the supplied values. so the name of the method is not with the naming convention.
            try
            {
                var command = new CreatePlanCommand
                {
                    DailyRate = request.DailyRate,
                    DurationInDays = request.DurationInDays,
                    IsActive = request.IsActive,
                    Name = request.Name,
                };
                var result = await _mediator.Send(command, cancellationToken);
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to create rental plan.");
                    return BadRequest(result);
                }
                _logger.LogInformation("Rental plan created successfully with ID: {Id}", result.Plan?.Id);
                return CreatedAtAction(nameof(GetPlanById), new { id = result.Plan?.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a rental plan.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates an existing rental plan.
        /// </summary>
        /// <param name="id">The ID of the rental plan to update.</param>
        /// <param name="request">The request with the updated plan details.</param>
        /// <param name="cancellationToken">Cancellation token for canceling the request.</param>
        /// <returns>An IActionResult that indicates the result of updating the plan.</returns>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlansCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PlansCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePlanAsync(int id, [FromBody] UpdatePlanRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new UpdatePlanCommand
                {
                    Name = request.Name,
                    IsActive = request.IsActive,
                    DailyRate = request.DailyRate,
                    DurationDays = request.DurationInDays,
                    Id = id                
                };

                var result = await _mediator.Send(command, cancellationToken);
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update rental plan: {Message}", result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("Rental plan updated successfully with ID: {Id}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the rental plan with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes a specific rental plan by ID.
        /// </summary>
        /// <param name="id">The ID of the rental plan to delete.</param>
        /// <param name="cancellationToken">Cancellation token for canceling the request.</param>
        /// <returns>An IActionResult that indicates whether the deletion was successful or not.</returns>
        [HttpDelete("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PlansCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PlansCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePlanAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeletePlanCommand { Id = id };
                var result = await _mediator.Send(command, cancellationToken);
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to delete rental plan: {Message}", result.Message);
                    return BadRequest(result);
                }

                _logger.LogInformation("Rental plan deleted successfully with ID: {Id}", id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the rental plan with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
