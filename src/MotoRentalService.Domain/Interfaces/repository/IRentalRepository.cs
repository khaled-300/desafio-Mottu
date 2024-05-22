using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces.repository
{
    public interface IRentalRepository
    {
        Task<Rental> AddRentAsync(Rental rental, CancellationToken cancellationToken);
        Task UpdateAsync(Rental rental, CancellationToken cancellationToken);
        Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Rental?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<Rental>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task DeleteRentAsync(Rental rental, CancellationToken cancellationToken);
        Task DeleteRentByUserIdAsync(int userId, CancellationToken cancellationToken);
    }
}
