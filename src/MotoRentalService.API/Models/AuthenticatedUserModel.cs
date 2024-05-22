using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.API.Models
{
    public class AuthenticatedUserModel
    {
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public int Id { get; set; }
    }
}
