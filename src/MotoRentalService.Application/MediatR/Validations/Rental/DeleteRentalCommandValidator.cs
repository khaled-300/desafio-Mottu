using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Rental;

namespace MotoRentalService.Application.MediatR.Validations.Rental
{
    public class DeleteRentalCommandValidator : AbstractValidator<DeleteRentalCommand>
    {
        public DeleteRentalCommandValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithName("Id");
        }
    }
}
