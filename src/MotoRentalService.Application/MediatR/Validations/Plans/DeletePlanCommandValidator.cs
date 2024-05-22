using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.Plans;

namespace MotoRentalService.Application.MediatR.Validations.Plans
{
    public class DeletePlanCommandValidator : AbstractValidator<DeletePlanCommand>
    {
        public DeletePlanCommandValidator()
        {
            RuleFor(m => m.Id)
                .NotEmpty().WithName("Id")
                .GreaterThan(0).WithName("Id");
        }
    }
}
