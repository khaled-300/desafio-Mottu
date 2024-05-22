using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Rental;
using MotoRentalService.Application.MediatR.Response.Rental;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.RentalHandlers
{
    public class SimulateTotalValueCommandHandler : BaseCommandHandler<SimulateTotalValueCommand, SimulateTotalValueResult>
    {
        private readonly IRentalService _rentalService;
        private readonly IValidator<SimulateTotalValueCommand> _validator;

        public SimulateTotalValueCommandHandler(IRentalService rentalService, IValidator<SimulateTotalValueCommand> validator) : base(validator)
        {
            _rentalService = rentalService;
            _validator = validator;
        }

        public override async Task<SimulateTotalValueResult> Handle(SimulateTotalValueCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var rental = await _rentalService.GetRentByIdAsync(request.Id, cancellationToken) ?? throw new ArgumentNullException(nameof(request.Id));
                var value = await _rentalService.CalculateFinalPriceAsync(rental, request.ReturnDate, cancellationToken);
                return new SimulateTotalValueResult(value, true, $"The simulation of the return date ${request.ReturnDate.ToLocalTime()} was executed successfully.");
            }
            catch (Exception ex)
            {

                return HandleException(ex);
            }
        }
    }
}
