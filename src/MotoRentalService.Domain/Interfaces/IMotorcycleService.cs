using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces
{
    public interface IMotorcycleService
    {
        Task<Motorcycle> CreateMotorcycleAsync(Motorcycle motorcycle, CancellationToken cancellationToken);
        Task DeleteMotorcycleAsync(int motoId, CancellationToken cancellationToken);
        Task UpdateMotorcycleAsync(Motorcycle motorcycle, CancellationToken cancellationToken);
        Task<Motorcycle?> GetMotorcycleById(int Id, CancellationToken cancellationToken);
    }
}