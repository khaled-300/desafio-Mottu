using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces.repository
{
    public interface IMotorcycleRepository
    {
        Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken);
        Task<int> CreateAsync(Motorcycle motorcycle, CancellationToken cancellationToken);
        Task<IEnumerable<Motorcycle>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Motorcycle?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task DeleteAsync(Motorcycle moto, CancellationToken cancellationToken);
        Task UpdateAsync(Motorcycle moto, CancellationToken cancellationToken);
    }
}
