using AutoMapper;
using FluentValidation;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.User;
using MotoRentalService.Application.MediatR.Response.DeliveryUser;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.MediatR.CommandHandlers.UserHandlers
{
    public class UpdateDeliveryUserCommandHandler : BaseCommandHandler<UpdateDeliveryUserCommand, DeliveryUserCommandResult>
    {
        private readonly IDeliveryUserService _deliveryUserService;
        private readonly IDeliveryUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateDeliveryUserCommand> _validator;
        public UpdateDeliveryUserCommandHandler(IMapper mapper, IDeliveryUserService deliveryUserService, IDeliveryUserRepository repository, IValidator<UpdateDeliveryUserCommand> validator): base(validator)
        {
            _validator = validator;
            _deliveryUserService = deliveryUserService;
            _repository = repository;
            _mapper = mapper;
        }

        public override async Task<DeliveryUserCommandResult> Handle(UpdateDeliveryUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return HandleValidationErrors(validationResult);
            }

            try
            {

                var userRequest = _mapper.Map<DeliveryUser>(request);
                var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (user == null) return new DeliveryUserCommandResult(success: false, message: "Delivery person not found");

                if (user.LicenseType != userRequest.LicenseType || user.LicenseType == default)
                {
                    user.LicenseType = userRequest.LicenseType;

                }

                if (user.LicenseNumber != userRequest.LicenseNumber || string.IsNullOrEmpty(user.LicenseNumber))
                {
                    user.LicenseNumber = userRequest.LicenseNumber;
                }

                var deliveryUser = await _deliveryUserService.UpdateUserLicenseImageAsync(user, request.LicenseImage, cancellationToken);
                var deliveryUserDto = _mapper.Map<DeliveryUserDto>(deliveryUser);
                return new DeliveryUserCommandResult(deliveryUserDto, true, "License image was updated successfully.");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
