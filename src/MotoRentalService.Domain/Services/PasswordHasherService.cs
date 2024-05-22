using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Domain.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(string password)
        {
            return BC.HashPassword(password);
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            return Verify(password, hashedPassword);
        }
    }
}
