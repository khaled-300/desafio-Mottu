using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Application.MediatR.Response.Moto;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.CommandHandlers.MotoHandlers
{
    public class DeleteMotoCommandHandler : BaseCommandHandler<DeleteMotoCommand, MotorcycleCommandResult>
    {
        private readonly IMotorcycleRepository _repository;
        private readonly IValidator<DeleteMotoCommand> _validator;

        public DeleteMotoCommandHandler(IMotorcycleRepository repository, IValidator<DeleteMotoCommand> validator) : base(validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public override async Task<MotorcycleCommandResult> Handle(DeleteMotoCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var result = new MotorcycleCommandResult();
                var motorcycle = await _repository.GetByIdAsync(request.Id, cancellationToken);

                if (motorcycle == null)
                {
                    result.AddError("Motorcycle was not found!");
                    return result;
                }

                await _repository.DeleteAsync(motorcycle, cancellationToken);
                result.SetSuccess("Motorcycle deleted successfully!");
                return result;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
