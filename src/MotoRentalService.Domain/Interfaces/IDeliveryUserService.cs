using Microsoft.AspNetCore.Http;
using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces
{
    public interface IDeliveryUserService
    {
        Task<DeliveryUser?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
        Task<DeliveryUser?> GetUserByUserIdAsync(int id, CancellationToken cancellationToken);
        Task<DeliveryUser> RegisterUserAsync(DeliveryUser deliveryPerson,IFormFile? image,CancellationToken cancellationToken);
        Task<DeliveryUser?> UpdateUserLicenseImageAsync(DeliveryUser deliveryPerson, IFormFile image, CancellationToken cancellationToken);
        Task DeleteUserAsync(int id, CancellationToken cancellationToken);
    }
}
