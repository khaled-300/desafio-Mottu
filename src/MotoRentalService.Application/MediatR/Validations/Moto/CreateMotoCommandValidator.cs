using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;

namespace MotoRentalService.Application.MediatR.Validations.Moto
{
    public class CreateMotoCommandValidator : AbstractValidator<CreateMotoCommand>
    {
        public CreateMotoCommandValidator()
        {
            RuleFor(m => m.Year).NotEmpty().WithName("Year").WithMessage("The year must not be empty.");
            RuleFor(m => m.Year).GreaterThan(0).WithName("Year").WithMessage("The year must be greater than 0.");
            RuleFor(m => m.Model).NotEmpty().WithName("Model").WithMessage("The model name must not be empty.");
            RuleFor(m => m.LicensePlate).NotEmpty().WithName("License Plate").WithMessage("The license plate must not be empty.");
        }
    }
}
