namespace MotoRentalService.Application.Request.Auth
{
    /// <summary>
    /// Represents the user login request.
    /// </summary>
    public class LoginAuthUserRequest
    {
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; }
    }
}
