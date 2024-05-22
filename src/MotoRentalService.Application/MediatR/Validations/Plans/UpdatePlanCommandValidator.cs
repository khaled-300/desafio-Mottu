using FluentValidation;
using MotoRentalService.Application.Extensions;
using MotoRentalService.Application.MediatR.Commands.Plans;

namespace MotoRentalService.Application.MediatR.Validations.Plans
{
    public class UpdatePlanCommandValidator : AbstractValidator<UpdatePlanCommand>
    {
        public UpdatePlanCommandValidator()
        {
            RuleFor(m => m.Id)
                .NotEmpty().WithName("Id")
                .GreaterThan(0).WithName("Id");

            RuleFor(m => m.Name)
                .NotEmpty().WithName("Name")
                .MinimumLength(3).WithName("Name")
                .MaximumLength(100).WithName("Name");

            RuleFor(m => m.DurationDays)
                .GreaterThan(0).WithName("Duration (Days)")
                .LessThanOrEqualTo(365).WithName("Duration (Days)");

            RuleFor(m => m.DailyRate)
                .NotEmpty().WithName("Daily Rate")
                .MustBeDecimal()
                .GreaterThan(0).WithName("Daily Rate");
        }
    }
}
