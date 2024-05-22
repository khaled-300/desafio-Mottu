using MediatR;
using Microsoft.AspNetCore.Http;
using MotoRentalService.Application.MediatR.Response.DeliveryUser;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.MediatR.Commands.User
{
    /// <summary>
    /// Command for updating delivery person information.
    /// </summary>
    public class UpdateDeliveryUserCommand : IRequest<DeliveryUserCommandResult>
    {
        /// <summary>
        /// The ID of the delivery person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The updated license number of the delivery person.
        /// </summary>
        /// <remarks>
        /// This property represents the updated license number of the delivery person.
        /// </remarks>
        public string LicenseNumber { get; set; }

        /// <summary>
        /// The updated license type of the delivery person.
        /// </summary>
        /// <remarks>
        /// This property represents the updated license type of the delivery person.
        /// </remarks>
        public LicenseType LicenseType { get; set; }

        /// <summary>
        /// The updated license image of the delivery person.
        /// </summary>
        /// <remarks>
        /// This property represents the updated license image of the delivery person.
        /// The image is expected to be in PNG or BMP format.
        /// </remarks>
        public IFormFile LicenseImage { get; set; }
    }
}
