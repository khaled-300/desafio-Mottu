using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Infrastructure.Repositories.Impl
{
    public class UserAuthenticationRepository : IUserAuthenticationRepository
    {
        private readonly AppDbContext _context;

        public UserAuthenticationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(Users user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
                throw new Exception("User was not found!");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Users?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
