using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Rental;

namespace MotoRentalService.Application.MediatR.Validations.Rental
{
    public class SimulateTotalValueCommandValidator : AbstractValidator<SimulateTotalValueCommand>
    {
        public SimulateTotalValueCommandValidator()
        {
            RuleFor(m => m.Id).NotEmpty().WithName("Id");
            RuleFor(m => m.ReturnDate)
                .NotEmpty().WithName("Return Date")
                .Must(BeAfterToday)
                .WithMessage("Return date must be a future date.");
        }

        private bool BeAfterToday(DateTime date)
        {
            return date > DateTime.Now;
        }
    }
}
