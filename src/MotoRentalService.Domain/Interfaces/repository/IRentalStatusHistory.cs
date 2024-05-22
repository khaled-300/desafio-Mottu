using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces.repository
{
    public interface IRentalStatusHistory
    {
        Task<RentalStatusHistory> AddRentalStatusHistoryAsync(RentalStatusHistory rentalStatus, CancellationToken cancellationToken);
    }
}
