using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.Response.Moto;
using MotoRentalService.Application.Request.Moto;

namespace MotoRentalService.API.Controllers
{
    /// <summary>
    /// Controller for managing motorcycles within the system, providing functionalities to create and manage motorcycle entries.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class MotorcyclesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MotorcyclesController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotorcyclesController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator to handle command and query dispatching.</param>
        /// <param name="logger">The logger to facilitate logging of activity within the controller.</param>
        public MotorcyclesController(IMediator mediator, ILogger<MotorcyclesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new motorcycle entry in the system.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /Create
        ///     {
        ///        "licensePlate": "XYZ1234",
        ///        "model": "Honda CB500",
        ///        "year": 2021
        ///     }
        /// 
        /// Upon successful creation, the motorcycle is persisted and can be retrieved or modified using its unique identifier.
        /// </remarks>
        /// <param name="request">The request containing the data needed to create a new motorcycle.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A status indicating success or failure along with the created motorcycle data.</returns>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MotorcycleCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MotorcycleCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateMotorcycleAsync([FromBody] CreateMotorcycleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateMotoCommand
                {
                    LicensePlate = request.LicensePlate,
                    Model = request.Model,
                    Year = request.Year
                };

                var result = await _mediator.Send(command, cancellationToken);
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetMotorcycle), new { motorcycleId = result.Motorcycle?.Id }, result.Motorcycle);
                }
                _logger.LogWarning("Failed to create motorcycle: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering a motorcycle.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves a paginated list of motorcycles.
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves a paginated list of motorcycles based on the provided page number and page size.
        /// </remarks>
        /// <param name="pageNumber">The page number to retrieve (1-based index).</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="ActionResult"/> containing the paginated list of motorcycles.</returns>
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<MotorcycleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMotorcyclesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                var motorcycles = await _mediator.Send(new GetMotoPageQuery(pageNumber, pageSize), cancellationToken);
                return Ok(motorcycles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching motorcycles.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves a motorcycle by its unique identifier.
        /// </summary>
        /// <param name="motorcycleId">The unique identifier of the motorcycle.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="ActionResult"/> containing the motorcycle details.</returns>
        [HttpGet("{motorcycleId}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MotorcycleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMotorcycle(int motorcycleId, CancellationToken cancellationToken)
        {
            try
            {
                var motorcycle = await _mediator.Send(new GetMotoByIdQuery(motorcycleId), cancellationToken);
                if (motorcycle == null)
                {
                    _logger.LogWarning("Motorcycle not found with ID: {MotorcycleId}", motorcycleId);
                    return NotFound($"No motorcycle found with ID: {motorcycleId}");
                }
                return Ok(motorcycle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching motorcycle details for ID: {MotorcycleId}", motorcycleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates the details of an existing motorcycle.
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        /// 
        /// ```json
        /// {
        ///   "licensePlate": "ABC-1234"
        /// }
        /// ```
        /// 
        /// </remarks>
        /// <param name="motorcycleId">The unique identifier of the motorcycle to update (in the URL path).</param>
        /// <param name="request">The request with the updated license plate information.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the update operation.</returns>
        [HttpPut("{motorcycleId}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MotorcycleCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MotorcycleCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateMotorcycleAsync(int motorcycleId, UpdateMotoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new UpdateMotoCommand
                {
                    Id = motorcycleId,
                    LicensePlate = request.LicensePlate
                };

                var result = await _mediator.Send(command, cancellationToken);
                if (result.Success)
                {
                    return Ok(result);
                }
                _logger.LogWarning("Failed to update motorcycle: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the motorcycle.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Deletes a motorcycle by its unique identifier.
        /// </summary>
        /// <param name="motorcycleId">The unique identifier of the motorcycle to delete.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the delete operation.</returns>
        [HttpDelete("{motorcycleId}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(MotorcycleCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MotorcycleCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMotorcycleAsync(int motorcycleId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new DeleteMotoCommand() { Id = motorcycleId }, cancellationToken);
                if (result.Success)
                {
                    return Ok(result);
                }
                _logger.LogWarning("Failed to delete motorcycle: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the motorcycle.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
