using AutoMapper;
using FluentValidation;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.User;
using MotoRentalService.Application.MediatR.Response.DeliveryUser;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.MediatR.CommandHandlers.UserHandlers
{
    public class CreateDeliveryUserCommandHandler : BaseCommandHandler<CreateUserDeliveryCommand, DeliveryUserCommandResult>
    {
        private readonly IDeliveryUserService _deliveryPersonService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateUserDeliveryCommand> _validator;
        public CreateDeliveryUserCommandHandler(IMapper mapper, IDeliveryUserService deliveryPersonService, IValidator<CreateUserDeliveryCommand> validator): base(validator)
        {
            _validator = validator;
             _mapper = mapper;
            _deliveryPersonService = deliveryPersonService;
        }

        public override async Task<DeliveryUserCommandResult> Handle(CreateUserDeliveryCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }
            try
            {
                var existingUser = await _deliveryPersonService.GetUserByUserIdAsync(request.AuthUserId, cancellationToken);
                if (existingUser != null) return new DeliveryUserCommandResult(success: false, message: "user has a record in the database please use the update or get endpoints.");

                var user = _mapper.Map<DeliveryUser>(request);
                var newUser = await _deliveryPersonService.RegisterUserAsync(user, request.LicenseImage, cancellationToken);
                var newUserDto = _mapper.Map<DeliveryUserDto>(newUser);
                return new DeliveryUserCommandResult(newUserDto, true, "Delivery user was created succsessfully.");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
