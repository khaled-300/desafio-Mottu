using AutoMapper;
using FluentValidation;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Application.MediatR.Response.Plans;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.CommandHandlers.PlansHandlers
{
    public class CreatePlanCommandHandler : BaseCommandHandler<CreatePlanCommand, PlansCommandResult>
    {
        private readonly IPlansRepository _plansRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePlanCommand> _validator;
        public CreatePlanCommandHandler(IMapper mapper, IPlansRepository plansRepository, IValidator<CreatePlanCommand> validator): base(validator)
        {
            _validator = validator; 
            _plansRepository = plansRepository;
            _mapper = mapper;
        }

        public override async Task<PlansCommandResult> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var rentalPlan = _mapper.Map<RentalPlans>(request);
                var newRentalplan = await _plansRepository.AddPlanAsync(rentalPlan, cancellationToken);
                var newRentalPlanDto = _mapper.Map<PlanDto>(newRentalplan);
                return new PlansCommandResult(newRentalPlanDto, true, "Plan was created successfully");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
