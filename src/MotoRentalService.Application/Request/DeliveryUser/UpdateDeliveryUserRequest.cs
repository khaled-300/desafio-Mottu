using Microsoft.AspNetCore.Http;
using MotoRentalService.Domain.Aggregates;
using MotoRentalService.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace MotoRentalService.Application.Request.DeliveryUser
{
    /// <summary>
    /// Represents the data needed to update delivery user
    /// </summary>
    public class UpdateDeliveryUserRequest
    {

        /// <summary>
        /// The updated license number of the delivery user.
        /// </summary>
        /// <remarks>
        /// This property represents the updated license number of the delivery user.
        /// </remarks>
        [Required]
        public string LicenseNumber { get; set; }

        /// <summary>
        /// The updated license type of the delivery user.
        /// </summary>
        /// <remarks>
        /// This property represents the updated license type of the delivery user.
        /// </remarks>
        [Required]
        public LicenseType LicenseType { get; set; }

        /// <summary>
        /// The updated license image of the delivery user.
        /// </summary>
        /// <remarks>
        /// This property represents the updated license image of the delivery user.
        /// The image is expected to be in PNG or BMP format.
        /// </remarks>
        [Required]
        public IFormFile LicenseImage { get; set; }
    }
}
