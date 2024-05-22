using MotoRentalService.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoRentalService.Domain.Entities
{
    /// <summary>
    /// Represents a user who can rent motorcycles.
    /// </summary>
    public class DeliveryUser : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the CNPJ (National Registry of Legal Entities) of the user.
        /// </summary>
        [Required]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ must be exactly 14 characters long.")]
        public string CNPJ { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the user.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the license number of the user.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "License number must be less than 50 characters long.")]
        public string LicenseNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of license held by the user.
        /// </summary>
        [Required]
        public LicenseType LicenseType { get; set; }

        /// <summary>
        /// Gets or sets the status of the user.
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the URL of the user's license image.
        /// </summary>
        [Url(ErrorMessage = "License image URL is not a valid URL.")]
        public string LicenseImageURL { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user's license image.
        /// </summary>
        public string LicenseImageFullName { get; set; }
        
        /// <summary>
        /// Gets or sets the userId which is ForeignKey of the table users
        /// </summary>
        [ForeignKey("Users")]
        public int UserId { get; set; }
        public Users User { get; set; }
    }
}
