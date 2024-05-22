using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Infrastructure.Repositories.Impl
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly AppDbContext _context;

        public MotorcycleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Motorcycle motorcycle, CancellationToken cancellationToken)
        {
            await _context.Motorcycles.AddAsync(motorcycle, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return motorcycle.Id;
        }

        public async Task<IEnumerable<Motorcycle>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.Motorcycles
                .OrderBy(m => m.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Motorcycle?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Motorcycles.FindAsync(id, cancellationToken);
        }

        public async Task<Motorcycle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken)
        {
            return await _context.Motorcycles
                .FirstOrDefaultAsync(m => m.LicensePlate == licensePlate, cancellationToken);
        }

        public async Task UpdateAsync(Motorcycle moto, CancellationToken cancellationToken)
        {
            _context.Motorcycles.Update(moto);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(Motorcycle moto, CancellationToken cancellationToken)
        {
            var existingMoto = await _context.Motorcycles.FindAsync(moto.Id);
            if (existingMoto == null)
            {
                throw new DbUpdateConcurrencyException("Attempted to delete an entity that does not exist in the store.");
            }
            _context.Motorcycles.Remove(moto);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
