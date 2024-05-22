using System.ComponentModel.DataAnnotations;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Domain.Entities
{
    /// <summary>
    /// Represents the authentication information for a user.
    /// </summary>
    public class Users : BaseEntity
    {
        /// <summary>
        /// Gets or sets the email associated with the user authentication.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password associated with the user authentication.
        /// </summary>
        [Required]
        [StringLength(255, MinimumLength = 8)]  // Assuming a minimum length for passwords
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the role associated with the user authentication.
        /// </summary>
        [Required]
        public UserRole Role { get; set; }
    }
}
