using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.Dtos
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for user information.
    /// </summary>
    public class UsersDto
    {
        /// <summary>
        /// Gets or sets the user ID 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the email address associated with the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's role.
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the user record.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the user record.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
