using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.User;

namespace MotoRentalService.Application.MediatR.Validations.DeliveryUser
{
    public class DeleteDeliveryUserCommandValidator : AbstractValidator<DeleteDeliveryUserCommand>
    {
        public DeleteDeliveryUserCommandValidator()
        {
            RuleFor(m => m.Id)
                .NotEmpty().WithName("Id")
                .GreaterThan(0).WithName("Id");
        }
    }
}
