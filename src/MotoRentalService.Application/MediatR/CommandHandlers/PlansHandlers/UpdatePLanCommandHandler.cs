using AutoMapper;
using FluentValidation;
using MediatR;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Application.MediatR.Response.Plans;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.CommandHandlers.PlansHandlers
{
    public class UpdatePLanCommandHandler : BaseCommandHandler<UpdatePlanCommand, PlansCommandResult>
    {
        private readonly IPlansRepository _plansRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdatePlanCommand> _validator;

        public UpdatePLanCommandHandler(IMapper mapper, IPlansRepository plansRepository, IValidator<UpdatePlanCommand> validator) : base(validator)
        {
            _validator = validator;
            _plansRepository = plansRepository;
            _mapper = mapper;
        }

        public override async Task<PlansCommandResult> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var rentalPlan = _mapper.Map<RentalPlans>(request);
                var existingPlan = await _plansRepository.GetPlanByIdAsync(rentalPlan.Id, cancellationToken);
                if (existingPlan == null)
                {
                    throw new KeyNotFoundException($"Plan was not found with Id: ${rentalPlan.Id}");
                }
                existingPlan.DurationInDays = request.DurationDays;
                existingPlan.DailyRate = request.DailyRate;
                existingPlan.IsActive = request.IsActive;
                existingPlan.Name = request.Name;

                var plan = await _plansRepository.UpdatePlanAsync(existingPlan, cancellationToken);
                var planDto = _mapper.Map<PlanDto>(plan);
                return new PlansCommandResult(planDto, true, "Plan was updated successfully");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
