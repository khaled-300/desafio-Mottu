using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Application.MediatR.Response.Plans;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.CommandHandlers.PlansHandlers
{
    public class DeletePlanCommandHandler : BaseCommandHandler<DeletePlanCommand, PlansCommandResult>
    {
        private readonly IPlansRepository _plansRepository;
        private readonly IValidator<DeletePlanCommand> _validator;

        public DeletePlanCommandHandler(IPlansRepository plansRepository, IValidator<DeletePlanCommand> validator): base(validator)
        {
            _validator = validator;
            _plansRepository = plansRepository;
        }

        public override async Task<PlansCommandResult> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                await _plansRepository.DeletePlanAsync(request.Id, cancellationToken);
                return new PlansCommandResult(success: true, message: "The Plan was deleted successfully");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
