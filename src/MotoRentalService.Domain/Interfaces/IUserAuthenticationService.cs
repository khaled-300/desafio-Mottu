using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces
{
    public interface IUserAuthenticationService
    {
        Task<string> RegisterUserAsync(Users user, CancellationToken cancellationToken);
        Task<string> AuthenticateUserAsync(string email, string password, CancellationToken cancellationToken);
        Task DeleteUserByIdAsync(int id, CancellationToken cancellationToken);
    }
}
