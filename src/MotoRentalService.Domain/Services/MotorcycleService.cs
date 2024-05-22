using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.Validations;
using System.ComponentModel.DataAnnotations;

namespace MotoRentalService.Domain.Services
{
    public class MotorcycleService : IMotorcycleService
    {
        private readonly IMotorcycleRepository _repository;

        public MotorcycleService(IMotorcycleRepository repository)
        {
            _repository = repository;
        }
        public async Task<Motorcycle> CreateMotorcycleAsync(Motorcycle motorcycle, CancellationToken cancellationToken)
        {
            await ValidateMotorcycleAsync(motorcycle, cancellationToken);
            var existingMotorcycle = await _repository.GetByLicensePlateAsync(motorcycle.LicensePlate, cancellationToken);
            if (existingMotorcycle != null)
            {
                throw new InvalidOperationException("License plate already in use.");
            }
            motorcycle.Id = await _repository.CreateAsync(motorcycle, cancellationToken);
            return motorcycle;
        }

        public async Task DeleteMotorcycleAsync(int motorcycleId, CancellationToken cancellationToken)
        {
            var motorcycle = await _repository.GetByIdAsync(motorcycleId, cancellationToken) ?? throw new KeyNotFoundException("Motorcycle not found.");
            await _repository.DeleteAsync(motorcycle, cancellationToken);
        }


        public async Task UpdateMotorcycleAsync(Motorcycle motorcycle, CancellationToken cancellationToken)
        {
            await _repository.UpdateAsync(motorcycle, cancellationToken);
        }

        public async Task<Motorcycle?> GetMotorcycleById(int Id, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(Id, cancellationToken);
        }

        private async Task ValidateMotorcycleAsync(Motorcycle deliveryPerson, CancellationToken cancellationToken)
        {
            var validationResult = await new MotorcycleValidator().ValidateAsync(deliveryPerson, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errorMessage);
            }
        }
    }
}
