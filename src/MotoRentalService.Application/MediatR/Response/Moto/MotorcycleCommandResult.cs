using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Response.Common;

namespace MotoRentalService.Application.MediatR.Response.Moto
{
    /// <summary>
    /// Represents the result of a command operation involving a motorcycle.
    /// </summary>
    public class MotorcycleCommandResult : ApiResult
    {
        /// <summary>
        /// Gets or sets the DTO containing motorcycle data.
        /// </summary>
        public MotorcycleDto? Motorcycle { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="MotorcycleCommandResult"/> class.
        /// </summary>
        public MotorcycleCommandResult()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MotorcycleCommandResult"/> class.
        /// </summary>
        /// <param name="moto">The motorcycle data transfer object.</param>
        /// <param name="success">Indicates whether the operation was successful.</param>
        /// <param name="message">A message describing the result of the operation.</param>
        public MotorcycleCommandResult(MotorcycleDto? moto = null,bool success = false, string message = null)
        {
            Motorcycle = moto;
            Success = success;
            Message = message;
        }
    }
}
