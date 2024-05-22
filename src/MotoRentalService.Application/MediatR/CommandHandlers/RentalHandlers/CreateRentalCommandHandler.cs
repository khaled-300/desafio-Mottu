using AutoMapper;
using FluentValidation;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Rental;
using MotoRentalService.Application.MediatR.Response.Rental;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.RentalHandlers
{
    public class CreateRentalCommandHandler : BaseCommandHandler<CreateRentalCommand, RentalCommandResult>
    {
        private readonly IRentalService _rentalService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateRentalCommand> _validator;

        public CreateRentalCommandHandler(IMapper mapper, IRentalService rentalService, IValidator<CreateRentalCommand> validator): base(validator)
        {
            _rentalService = rentalService;
            _mapper = mapper;
            _validator = validator;
        }

        public override async Task<RentalCommandResult> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var rental = _mapper.Map<Rental>(request);
                var newRental = await _rentalService.RentMotorcycleAsync(rental, cancellationToken);

                if (newRental == null)
                {
                    return new RentalCommandResult(success: false, message: "Failed to create rental. Please check details.");
                }

                var newRentalDto = _mapper.Map<RentalDto>(newRental);
                return new RentalCommandResult(newRentalDto, true, "Rent was created successfully");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
