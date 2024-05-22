using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Rental;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.Response.Rental;
using MotoRentalService.Application.Request.Rental;


namespace MotoRentalService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RentalController> _logger;

        public RentalController(IMediator mediator, ILogger<RentalController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RentalCommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RentalCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRentalAsync([FromBody] CreateRentalRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateRentalCommand
                {
                    EndDate = request.EndDate,
                    MotorcycleId = request.MotorcycleId,
                    RentalPlanId = request.RentalPlanId,
                    StartDate = request.StartDate,
                    UserId = request.UserId
                };

                var result = await _mediator.Send(command, cancellationToken);
                if (result.Success)
                    return CreatedAtAction(nameof(GetRentalById), new { rentalId = result.Rental?.Id }, result);

                _logger.LogWarning("Failed to createnew rental agreement: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating rental.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves a rental by its ID.
        /// </summary>
        /// <param name="rentalId">The ID of the rental to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token for canceling the request.</param>
        /// <returns>An IActionResult containing the rental details or an error message.</returns>
        [HttpGet("{rentalId}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RentalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RentalCommandResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRentalById(int rentalId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(new GetRentalByIdQuery { RentalId = rentalId }, cancellationToken);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching rental details.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        /// <summary>
        /// Simulates updating the return date of a rental contact.
        /// </summary>
        /// <param name="request">The request containing the rental ID and the new return date.</param>
        /// <param name="cancellationToken">Cancellation token for canceling the request.</param>
        /// <returns>An IActionResult containing the result of the operation with the calculated total value of the rental contract.</returns>
        [HttpPost("simulate-return-date")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(SimulateTotalValueResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimulateTotalValueResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SimulateReturnDateAsync([FromBody] SimulateTotalValueRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new SimulateTotalValueCommand
                {
                    Id = request.Id,
                    ReturnDate = request.ReturnDate
                };
                var result = await _mediator.Send(command, cancellationToken);
                if (result.Success)
                    return Ok(result);

                _logger.LogError("Failed to caluclate the total value of the rental contact with id: {0}", request.Id);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating return date of rental.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
