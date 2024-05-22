using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Interfaces.repository
{
    public interface IPlansRepository
    {
        Task<RentalPlans> AddPlanAsync(RentalPlans rentalPlan, CancellationToken cancellationToken);
        Task<IEnumerable<RentalPlans>> GetAllAsync(CancellationToken cancellationToken);
        Task<RentalPlans?> GetPlanByIdAsync(int id, CancellationToken cancellationToken);
        Task<RentalPlans> UpdatePlanAsync(RentalPlans rentalPlan, CancellationToken cancellationToken);
        Task DeletePlanAsync(int id, CancellationToken cancellationToken);
    }
}
