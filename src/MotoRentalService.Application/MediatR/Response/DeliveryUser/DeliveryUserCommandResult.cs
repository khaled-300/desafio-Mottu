using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Response.Common;

namespace MotoRentalService.Application.MediatR.Response.DeliveryUser
{
    /// <summary>
    /// Represents the result of a command operation for delivery user actions.
    /// </summary>
    public class DeliveryUserCommandResult : ApiResult
    {
        /// <summary>
        /// Gets or sets the DTO containing delivery person data.
        /// </summary>
        public DeliveryUserDto? DeliveryUser { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryUserCommandResult"/> class.
        /// </summary>
        public DeliveryUserCommandResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryUserCommandResult"/> class.
        /// </summary>
        /// <param name="user">The delivery person DTO to include in the result.</param>
        /// <param name="success">Indicates whether the operation was successful.</param>
        /// <param name="message">A message describing the result of the operation.</param>
        public DeliveryUserCommandResult(DeliveryUserDto? user = null, bool success = false, string message = null)
        {
            DeliveryUser = user;
            Success = success;
            Message = message;
        }
    }
}
