using AutoMapper;
using FluentValidation;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Application.MediatR.Response.Moto;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.MotoHandlers
{
    public class UpdateMotoCommandHandler : BaseCommandHandler<UpdateMotoCommand, MotorcycleCommandResult>
    {
        private readonly IMapper _mapper;
        private readonly IMotorcycleService _motorcycleService;
        private readonly IValidator<UpdateMotoCommand> _validator;

        public UpdateMotoCommandHandler(IMapper mapper, IMotorcycleService motorcycleService, IValidator<UpdateMotoCommand> validator): base(validator)
        {
            _mapper = mapper;
            _motorcycleService = motorcycleService;
            _validator = validator;
        }

        public override async Task<MotorcycleCommandResult> Handle(UpdateMotoCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var result = new MotorcycleCommandResult();
                var motorcycleToUpdate = await _motorcycleService.GetMotorcycleById(request.Id, cancellationToken);

                if (motorcycleToUpdate == null)
                {
                    result.AddError("Motorcycle not found.");
                    return result;
                }

                motorcycleToUpdate.LicensePlate = request.LicensePlate;


                await _motorcycleService.UpdateMotorcycleAsync(motorcycleToUpdate, cancellationToken);
                var updatedMotorcycle = await _motorcycleService.GetMotorcycleById(request.Id, cancellationToken);
                var updatedDto = _mapper.Map<MotorcycleDto>(updatedMotorcycle);
                result.SetSuccess("Motorcycle updated successfully.");
                result.Motorcycle = updatedDto;
                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
