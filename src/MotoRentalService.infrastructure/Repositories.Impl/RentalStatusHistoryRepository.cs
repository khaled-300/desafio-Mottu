using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Infrastructure.Repositories.Impl
{
    public class RentalStatusHistoryRepository : IRentalStatusHistory
    {
        private readonly AppDbContext _context;

        public RentalStatusHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.RentalStatusHistory> AddRentalStatusHistoryAsync(Domain.Entities.RentalStatusHistory rentalStatus, CancellationToken cancellationToken)
        {
            await _context.RentalStatusHistories.AddAsync(rentalStatus, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return rentalStatus;
        }
    }
}
