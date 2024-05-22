using FluentValidation;
using MotoRentalService.Application.MediatR.Commands.User;
using MotoRentalService.Application.MediatR.Response.DeliveryUser;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.UserHandlers
{
    public class DeleteDeliveryUserCommandHandler : BaseCommandHandler<DeleteDeliveryUserCommand, DeliveryUserCommandResult>
    {
        private readonly IDeliveryUserService _deliveryUserService;
        private readonly IUserAuthenticationService _UserAuthenticationService;
        private readonly IRentalService _rentalService;
        private readonly IValidator<DeleteDeliveryUserCommand> _validator;

        public DeleteDeliveryUserCommandHandler(IDeliveryUserService deliveryUserService, IUserAuthenticationService userAuthenticationService, IRentalService rentalService, IValidator<DeleteDeliveryUserCommand> validator): base(validator)
        {
            _validator = validator;
            _deliveryUserService = deliveryUserService;
            _UserAuthenticationService = userAuthenticationService;
            _rentalService = rentalService;
        }

        public override async Task<DeliveryUserCommandResult> Handle(DeleteDeliveryUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {
                var user = await _deliveryUserService.GetUserByIdAsync(request.Id, cancellationToken);

                if (user != null)
                {
                    var rental = await _rentalService.GetRentByUserIdAsync(user.UserId, cancellationToken);
                    if (rental != null)
                    {
                        if (rental.Status == Domain.ValueObjects.RentalStatus.Active)
                        {
                            return new DeliveryUserCommandResult(success: false, message: "Delivery user has an active rental contract.");
                        }
                        await _rentalService.DeleteRentalByUserIdAsync(request.Id, cancellationToken);
                    }
                    await _deliveryUserService.DeleteUserAsync(request.Id, cancellationToken);
                    await _UserAuthenticationService.DeleteUserByIdAsync(user.Id, cancellationToken);
                    return new DeliveryUserCommandResult(success: true, message: "Delivery user was deleted successfully.");
                }
                else
                {
                    return new DeliveryUserCommandResult(success: false, message: "User was not found.");
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
