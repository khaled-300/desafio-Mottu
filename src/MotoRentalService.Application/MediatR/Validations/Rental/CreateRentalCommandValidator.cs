using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Rental;

namespace MotoRentalService.Application.MediatR.Validations.Rental
{
    public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
    {
        public CreateRentalCommandValidator()
        {
            RuleFor(r => r.UserId).NotEmpty().WithMessage("Delivery user should not be empty.");
            RuleFor(r => r.UserId).GreaterThan(0).WithMessage("Delivery user should be greater than 0.");
            RuleFor(r => r.MotorcycleId).NotEmpty().WithMessage("Motorcycle id should not be empty.");
            RuleFor(r => r.RentalPlanId).NotEmpty().WithMessage("Rental plan id should not be empty.");

            // Start date validation
            RuleFor(r => r.StartDate).GreaterThan(DateTime.UtcNow)
              .WithMessage("Start date must be after today.");

            // End date validation
            RuleFor(r => r.EndDate).GreaterThan(r => r.StartDate)
              .WithMessage("End date must grater than start date.");
        }
    }
}
