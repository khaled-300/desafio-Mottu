using FluentValidation;
using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Validations
{
    public class RentalValidator : AbstractValidator<Rental>
    {
        public RentalValidator()
        {
            RuleFor(r => r.UserId).NotEmpty().WithMessage("Delivery user should not be empty.");
            RuleFor(r => r.MotorcycleId).NotEmpty().WithMessage("Motorcycle id should not be empty.");
            RuleFor(r => r.RentalPlanId).NotEmpty().WithMessage("Rental plan id should not be empty.");

            // Start date validation
            RuleFor(r => r.StartDate).GreaterThan(DateTime.UtcNow)
              .WithMessage("Start date must be after today.");

            // Expected end date validation (assuming related to rental plan)
            RuleFor(r => r.ExpectedEndDate).GreaterThan(r => r.StartDate)
              .WithMessage("Expected end date must be after start date.");
        }
    }
}
