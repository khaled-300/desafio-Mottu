using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;

namespace MotoRentalService.Application.MediatR.Validations.Moto
{
    public class UpdateMotoCommandValidator : AbstractValidator<UpdateMotoCommand>
    {
        public UpdateMotoCommandValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithName("Id");
            RuleFor(m => m.Id).GreaterThan(0).WithName("Id").WithMessage("Id must be grator than 0");
            RuleFor(m => m.LicensePlate).NotEmpty().WithName("License Plate");
        }
    }
}
