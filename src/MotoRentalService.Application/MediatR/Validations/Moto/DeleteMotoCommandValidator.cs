using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;

namespace MotoRentalService.Application.MediatR.Validations.Moto
{
    public class DeleteMotoCommandValidator : AbstractValidator<DeleteMotoCommand>
    {
        public DeleteMotoCommandValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithName("Id");
        }
    }
}
