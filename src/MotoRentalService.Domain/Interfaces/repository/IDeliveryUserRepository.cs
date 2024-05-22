using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces.repository
{
    public interface IDeliveryUserRepository
    {
        Task<DeliveryUser?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<DeliveryUser?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<DeliveryUser>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<DeliveryUser> AddAsync(DeliveryUser deliveryPerson, CancellationToken cancellationToken);
        Task UpdateAsync(DeliveryUser deliveryPerson, CancellationToken cancellationToken);
        Task DeleteByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> ExistsByCNPJAsync(string cnpj, CancellationToken cancellationToken);
        Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken);
    }
}
