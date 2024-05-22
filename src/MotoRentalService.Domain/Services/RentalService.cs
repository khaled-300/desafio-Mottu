using FluentValidation;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.Validations;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Domain.Services
{
    public class RentalService : IRentalService
    {
        private readonly IDeliveryUserRepository _deliveryUserRepository;
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IRentalRepository _rentalRepository;
        private readonly IPlansRepository _plansRepository;
        private readonly IRentalStatusHistory _rentalStatusHistory;

        public RentalService(IDeliveryUserRepository deliveryPersonRepository,
                             IMotorcycleRepository motorcycleRepository,
                             IRentalRepository rentalRepository,
                             IPlansRepository plansRepository,
                             IRentalStatusHistory rentalStatusHistory)
        {
            _deliveryUserRepository = deliveryPersonRepository;
            _motorcycleRepository = motorcycleRepository;
            _rentalRepository = rentalRepository;
            _plansRepository = plansRepository;
            _rentalStatusHistory = rentalStatusHistory;
        }

        public async Task<Rental> RentMotorcycleAsync(Rental rental, CancellationToken cancellationToken)
        {
            if (rental == null) throw new ArgumentNullException(nameof(rental));
            rental.ExpectedEndDate = rental.StartDate.AddDays((rental.EndDate - rental.StartDate).Days);
            await ValidateRentalAsync(rental, cancellationToken);

            var deliveryPerson = await EnsureDeliveryPersonCanRentAsync(rental.UserId, cancellationToken);
            var motorcycle = await EnsureMotorcycleAvailableAsync(rental.MotorcycleId, cancellationToken);
            var plan = await _plansRepository.GetPlanByIdAsync(rental.RentalPlanId, cancellationToken)
                ?? throw new KeyNotFoundException($"No plan found with ID {rental.RentalPlanId}");
            rental.AssignRentalDetails(deliveryPerson.Id, motorcycle.Id, plan.Id, plan.DailyRate, rental.StartDate.AddDays(plan.DurationInDays), RentalStatus.Pending);
            rental.TotalPrice = await CalculateBaseTotalPriceAsync(rental, cancellationToken);
            motorcycle.MarkAsRented();
            await _rentalRepository.AddRentAsync(rental, cancellationToken);
            await _motorcycleRepository.UpdateAsync(motorcycle, cancellationToken);
            await _rentalStatusHistory.AddRentalStatusHistoryAsync(new RentalStatusHistory { Status = rental.Status, RentalId = rental.Id, StatusChangedDate = DateTime.UtcNow }, cancellationToken);

            return rental;
        }

        public async Task<decimal> CalculateFinalPriceAsync(Rental rental, DateTime returnDate, CancellationToken cancellationToken)
        {
            // Fetch the motorcycle and rental plan details.
            var motorcycle = await _motorcycleRepository.GetByIdAsync(rental.MotorcycleId, cancellationToken);
            if (motorcycle == null)
            {
                throw new InvalidOperationException("Motorcycle not found.");
            }

            var rentalPlan = await _plansRepository.GetPlanByIdAsync(rental.RentalPlanId, cancellationToken);
            if (rentalPlan == null)
            {
                throw new InvalidOperationException("Rental plan assigned to the motorcycle not found.");
            }

            // Set the expected end date based on the plan duration and calculate base cost.
            rental.ExpectedEndDate = rental.StartDate.AddDays(rentalPlan.DurationInDays);
            decimal baseCost = rentalPlan.DailyRate * rentalPlan.DurationInDays;
            decimal totalCost = baseCost;

            // Determine if the return is early or late.
            if (returnDate < rental.ExpectedEndDate)
            {
                // Early return calculations.
                int daysEarly = (rental.ExpectedEndDate - returnDate).Days;
                decimal finePercentage = rentalPlan.DurationInDays == 7 ? 0.2m : 0.4m;
                decimal unpaidDaysCost = daysEarly * rentalPlan.DailyRate;
                decimal fineAmount = unpaidDaysCost * finePercentage;

                totalCost = baseCost - unpaidDaysCost + fineAmount; // Base cost minus the days not used plus the fine.
            }
            else if (returnDate > rental.ExpectedEndDate)
            {
                // Late return calculations.
                int daysLate = (returnDate - rental.ExpectedEndDate).Days;
                decimal additionalCharge = daysLate * 50m; // R$50 for each additional day.
                totalCost += additionalCharge;
            }

            return totalCost;
        }

        public async Task<decimal> CalculateBaseTotalPriceAsync(Rental rental, CancellationToken cancellationToken)
        {
            // Fetch the motorcycle and rental plan details.
            var motorcycle = await _motorcycleRepository.GetByIdAsync(rental.MotorcycleId, cancellationToken);
            if (motorcycle == null)
            {
                throw new InvalidOperationException("Motorcycle not found.");
            }

            var rentalPlan = await _plansRepository.GetPlanByIdAsync(rental.RentalPlanId, cancellationToken);
            if (rentalPlan == null)
            {
                throw new InvalidOperationException("Rental plan assigned to the motorcycle not found.");
            }

            // Set the expected end date based on the plan duration and calculate base cost.
            rental.ExpectedEndDate = rental.StartDate.AddDays(rentalPlan.DurationInDays);
            decimal baseCost = rentalPlan.DailyRate * rentalPlan.DurationInDays;
            decimal totalCost = baseCost;

            return totalCost;
        }

        public async Task<Rental> MarkRentalAsCompletedAsync(int rentalId, CancellationToken cancellationToken)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId, cancellationToken);
            if (rental == null)
            {
                throw new Exception("Rental not found.");
            }

            rental.Status = RentalStatus.Completed;
            await _rentalRepository.UpdateAsync(rental, cancellationToken);
            await _rentalStatusHistory.AddRentalStatusHistoryAsync(new RentalStatusHistory { RentalId = rental.Id, Status = rental.Status, StatusChangedDate = DateTime.UtcNow }, cancellationToken);
            return rental;
        }

        public Task<Rental?> GetRentByIdAsync(int id, CancellationToken cancellationToken)
        {
            return _rentalRepository.GetByIdAsync(id, cancellationToken);
        }


        private async Task ValidateRentalAsync(Rental rental, CancellationToken cancellationToken)
        {
            var validator = new RentalValidator();
            var validationResult = await validator.ValidateAsync(rental, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
        private async Task<DeliveryUser> EnsureDeliveryPersonCanRentAsync(int deliveryUserId, CancellationToken cancellationToken)
        {
            var deliveryPerson = await _deliveryUserRepository.GetByIdAsync(deliveryUserId, cancellationToken)
                ?? throw new KeyNotFoundException($"Delivery person with ID {deliveryUserId} not found.");

            if (deliveryPerson.LicenseType != LicenseType.A && deliveryPerson.LicenseType != LicenseType.AB)
                throw new InvalidOperationException("Delivery person must have license category A or AB to rent.");

            return deliveryPerson;
        }
        private async Task<Motorcycle> EnsureMotorcycleAvailableAsync(int motorcycleId, CancellationToken cancellationToken)
        {
            var motorcycle = await _motorcycleRepository.GetByIdAsync(motorcycleId, cancellationToken)
                ?? throw new KeyNotFoundException($"Motorcycle with ID {motorcycleId} not found.");

            if (motorcycle.IsRented)
                throw new InvalidOperationException("Motorcycle is currently not available for rent.");

            return motorcycle;
        }

        public async Task DeleteRentalByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            await _rentalRepository.DeleteRentByUserIdAsync(userId, cancellationToken);
        }

        public Task<Rental?> GetRentByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return _rentalRepository.GetByUserIdAsync(userId, cancellationToken);
        }
    }
}