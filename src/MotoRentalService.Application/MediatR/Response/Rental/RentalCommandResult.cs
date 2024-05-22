using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Response.Common;
using MotoRentalService.Application.MediatR.Response.Moto;

namespace MotoRentalService.Application.MediatR.Response.Rental
{
    /// <summary>
    /// Represents the result of a rental command operation.
    /// </summary>
    public class RentalCommandResult : ApiResult
    {
        /// <summary>
        /// Gets or sets the DTO representing the rental details.
        /// </summary>
        public RentalDto? Rental { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RentalCommandResult"/> class.
        /// </summary>
        public RentalCommandResult()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RentalCommandResult"/> class.
        /// </summary>
        /// <param name="rental">The DTO containing rental details.</param>
        /// <param name="success">A value indicating whether the rental operation was successful.</param>
        /// <param name="message">A message describing the result of the rental operation.</param>
        public RentalCommandResult(RentalDto? rental = null, bool success = false, string message = null)
        {
            Success = success;
            Message = message;
            Rental = rental;
        }
    }
}
