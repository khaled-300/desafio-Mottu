using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Domain.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(int userId, string email, UserRole userRole, CancellationToken cancellationToken);
    }
}