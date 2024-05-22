using Microsoft.AspNetCore.Http;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.Dtos
{
    /// <summary>
    /// Represents the data transfer object for a delivery user.
    /// </summary>
    public class DeliveryUserDto
    {
        /// <summary>
        ///  Unique identifier for the delivery user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///  Name of the delivery user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  CNPJ number (company identification number in Brazil) for the delivery user.
        /// </summary>
        public string CNPJ { get; set; }

        /// <summary>
        ///  Date of birth of the delivery user.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        ///  License number of the delivery user.
        /// </summary>
        public string LicenseNumber { get; set; }

        /// <summary>
        ///  Type of license held by the delivery user.
        /// </summary>
        public LicenseType LicenseType { get; set; }

        /// <summary>
        /// License image binary (file)
        /// </summary>
        public IFormFile? LicenseImage { get; set; }

        /// <summary>
        ///  URL for the image of the license.
        /// </summary>
        public string LicenseImageURL { get; set; }

        /// <summary>
        ///  Creation date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///  Last update date.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
