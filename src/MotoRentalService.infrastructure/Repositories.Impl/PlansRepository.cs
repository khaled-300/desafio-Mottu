using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Infrastructure.Repositories.Impl
{
    public class PlansRepository : IPlansRepository
    {
        private readonly AppDbContext _context;

        public PlansRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RentalPlans> AddPlanAsync(RentalPlans rentalPlan, CancellationToken cancellationToken)
        {
            _context.RentalPlans.Add(rentalPlan);
            await _context.SaveChangesAsync(cancellationToken);
            return rentalPlan;
        }

        public async Task<RentalPlans?> GetPlanByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.RentalPlans.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<RentalPlans> UpdatePlanAsync(RentalPlans rentalPlan, CancellationToken cancellationToken)
        {
            _context.RentalPlans.Update(rentalPlan);
            await _context.SaveChangesAsync(cancellationToken);
            return rentalPlan;
        }

        public async Task DeletePlanAsync(int id, CancellationToken cancellationToken)
        {
            var rentalPlan = await _context.RentalPlans.FindAsync(new object[] { id }, cancellationToken);
            if (rentalPlan == null)
                throw new Exception("Plan was not found!");

            _context.RentalPlans.Remove(rentalPlan);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<RentalPlans>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.RentalPlans.ToListAsync(cancellationToken);
        }
    }
}
