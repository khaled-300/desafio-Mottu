using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Infrastructure.Repositories.Impl
{
    public class DeliveryUserRepository : IDeliveryUserRepository
    {
        private readonly AppDbContext _context;

        public DeliveryUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryUser?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.DeliveryUser.FindAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<DeliveryUser>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.DeliveryUser
              .OrderBy(dp => dp.Id)
              .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync(cancellationToken);
        }

        public async Task<DeliveryUser> AddAsync(DeliveryUser user, CancellationToken cancellationToken)
        {
            await _context.DeliveryUser.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user; // Return the created entity
        }

        public async Task UpdateAsync(DeliveryUser user, CancellationToken cancellationToken)
        {
            _context.DeliveryUser.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _context.DeliveryUser.FindAsync(id, cancellationToken);
            if (user == null)
            {
                throw new Exception("Delivery user was not found");
            }

            _context.DeliveryUser.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCNPJAsync(string cnpj, CancellationToken cancellationToken)
        {
            return await _context.DeliveryUser.AnyAsync(dp => dp.CNPJ == cnpj, cancellationToken);
        }

        public async Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken)
        {
            return await _context.DeliveryUser.AnyAsync(dp => dp.LicenseNumber == licenseNumber, cancellationToken);
        }

        public Task<DeliveryUser?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return _context.DeliveryUser.FirstOrDefaultAsync(user  => user.UserId == userId, cancellationToken);
        }
    }
}
