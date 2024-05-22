using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Infrastructure.Repositories.Impl
{
    public class RentalRepository : IRentalRepository
    {
        private readonly AppDbContext _context;

        public RentalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Rental> AddRentAsync(Rental rental, CancellationToken cancellationToken)
        {
            await _context.Rentals.AddAsync(rental, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return rental;
        }

        public async Task UpdateAsync(Rental rental, CancellationToken cancellationToken)
        {
            _context.Rentals.Update(rental);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Rental?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Rentals
              .Include(r => r.RentalPlan)
              .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Rental>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.Rentals
              .Include(r => r.RentalPlan)
              .OrderBy(r => r.Id)
              .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync(cancellationToken);
        }

        public async Task DeleteRentAsync(Rental rental, CancellationToken cancellationToken)
        {
            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRentByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var rental = await _context.Rentals.FindAsync(new object[] { userId }, cancellationToken);
            if (rental == null) throw new Exception($"rental was not found with the userId: {userId}");
            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Rental?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await _context.Rentals.FindAsync(new object[] { userId }, cancellationToken);
        }
    }
}
