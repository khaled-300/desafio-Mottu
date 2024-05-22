using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces.repository
{
    public interface IUserAuthenticationRepository
    {
        Task AddUserAsync(Users user, CancellationToken cancellationToken);
        Task<Users?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task DeleteUserByIdAsync(int id, CancellationToken cancellationToken);
    }
}
