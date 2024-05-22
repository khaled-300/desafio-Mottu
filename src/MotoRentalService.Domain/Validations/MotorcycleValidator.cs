using FluentValidation;
using MotoRentalService.Domain.Entities;

namespace MotoRentalService.Domain.Validations
{
    public class MotorcycleValidator : AbstractValidator<Motorcycle>
    {
        public MotorcycleValidator()
        {
            RuleFor(m => m.Year).NotEmpty().WithName("Year");
            RuleFor(m => m.Model).NotEmpty().WithName("Model");
            RuleFor(m => m.LicensePlate).NotEmpty().WithName("License Plate");
        }
    }
}
