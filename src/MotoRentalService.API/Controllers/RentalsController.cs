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
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RentalsController> _logger;

        public RentalsController(IMediator mediator, ILogger<RentalsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new rental agreement.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a JSON request containing details for the new rental agreement. 
        /// 
        /// Sample request:
        /// 
        /// ```json
        /// {
        ///   "motorcycleId": 1,
        ///   "userId": 2,
        ///   "startDate": "2024-05-24T00:00:00Z",
        ///   "endDate": "2024-05-27T00:00:00Z",
        ///   "rentalPlanId": 3
        /// }
        /// ```
        /// 
        /// </remarks>
        /// <param name="request">The request object containing rental agreement details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An IActionResult representing the HTTP response.</returns>
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
        /// Simulates updating the return date of a rental contract and calculates the total value based on the new return date.
        /// </summary>
        /// <remarks>
        /// This endpoint expects a JSON request containing the rental ID and the new desired return date. 
        /// 
        /// It performs a simulation of updating the return date without actually modifying the rental contract. 
        /// The total value of the rental is calculated based on the provided new return date.
        /// 
        /// Sample request:
        /// 
        /// ```json
        /// {
        ///   "id": 123,
        ///   "returnDate": "2024-05-28T00:00:00Z"
        /// }
        /// ```
        /// 
        /// </remarks>
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
