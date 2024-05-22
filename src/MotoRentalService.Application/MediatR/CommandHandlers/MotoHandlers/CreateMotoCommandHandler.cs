using AutoMapper;
using FluentValidation;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Application.MediatR.Response.Moto;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.MotoHandlers
{
    public class CreateMotoCommandHandler : BaseCommandHandler<CreateMotoCommand, MotorcycleCommandResult>
    {
        private readonly IMotorcycleService _motorcycleService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateMotoCommand> _validator;
        public CreateMotoCommandHandler(IMapper mapper, IMotorcycleService motorcycleService, IValidator<CreateMotoCommand> validator) : base(validator)
        {
            _validator = validator;
            _mapper = mapper;
            _motorcycleService = motorcycleService;
        }

        public override async Task<MotorcycleCommandResult> Handle(CreateMotoCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var motorcycle = _mapper.Map<Motorcycle>(request);
                var newMotorcycle = await _motorcycleService.CreateMotorcycleAsync(motorcycle, cancellationToken);
                var created = _mapper.Map<MotorcycleDto>(newMotorcycle);

                var result = new MotorcycleCommandResult();
                if (created == null)
                {
                    result.AddError("Motorcycle was not registered successfully.");
                    return result;
                }

                result.SetSuccess("Motorcycle registered successfully.");
                result.Motorcycle = created;

                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
