using Microsoft.AspNetCore.Http;
using MotoRentalService.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace MotoRentalService.Application.Request.DeliveryUser
{
    /// <summary>
    /// Represents the data needed for delivery user.
    /// </summary>
    public class CreateDeliveryUserRequest
    {
        /// <summary>
        ///  Name of the delivery person.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  CNPJ number (company identification number in Brazil) for the delivery person.
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        ///  Date of birth of the delivery person.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        ///  License number of the delivery person.
        /// </summary>
        public string LicenseNumber { get; set; }

        /// <summary>
        ///  Type of license held by the delivery person.
        /// </summary>
        public LicenseType LicenseType { get; set; }

        /// <summary>
        /// License image binary (file)
        /// </summary>
        public IFormFile? LicenseImage { get; set; }
    }
}
