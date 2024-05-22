using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.Request.Auth
{
    /// <summary>
    /// represent the authentication user request data
    /// </summary>
    public class CreateAuthUserRequest
    {
        /// <summary>
        /// User Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// User Role
        /// </summary>
        public UserRole Role { get; set; }
    }
}
