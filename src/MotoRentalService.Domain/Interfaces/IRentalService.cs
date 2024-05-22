using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces
{
    public interface IRentalService
    {
        Task<Rental> RentMotorcycleAsync(Rental rental, CancellationToken cancellationToken);
        Task<decimal> CalculateFinalPriceAsync(Rental rental, DateTime returnDate, CancellationToken cancellationToken);
        Task<decimal> CalculateBaseTotalPriceAsync(Rental rental, CancellationToken cancellationToken);
        Task<Rental> MarkRentalAsCompletedAsync(int rentalId, CancellationToken cancellationToken);
        Task<Rental?> GetRentByIdAsync(int id, CancellationToken cancellationToken);
        Task<Rental?> GetRentByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task DeleteRentalByUserIdAsync(int userId, CancellationToken cancellationToken);
    }
}
