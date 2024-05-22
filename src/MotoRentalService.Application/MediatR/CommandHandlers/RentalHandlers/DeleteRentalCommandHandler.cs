using AutoMapper;
using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Rental;
using MotoRentalService.Application.MediatR.Response.Rental;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.RentalHandlers
{
    public class DeleteRentalCommandHandler : BaseCommandHandler<DeleteRentalCommand, RentalCommandResult>
    {
        private readonly IRentalService _rentalService;
        private readonly IMapper _mapper;
        private readonly IValidator<DeleteRentalCommand> _validator;

        public DeleteRentalCommandHandler(IMapper mapper, IRentalService rentalService, IValidator<DeleteRentalCommand> validator) : base(validator)
        {
            _validator = validator;
            _rentalService = rentalService;
            _mapper = mapper;
        }
        public override async Task<RentalCommandResult> Handle(DeleteRentalCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var rental = _mapper.Map<Rental>(request);
                await _rentalService.MarkRentalAsCompletedAsync(rental.Id, cancellationToken);
                return new RentalCommandResult(success: true, message: "Plan was marked as completed successfully");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
