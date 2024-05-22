using AutoMapper;
using Moq;
using MotoRentalService.Application.MediatR.CommandHandlers.UserHandlers;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Application.MediatR.Commands.User;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace MotoRentalService.Application.Tests.Handlers.UserHandlers
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IDeliveryUserService> _deliveryUserServiceMock;
        private readonly Mock<IDeliveryUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateDeliveryUserCommandHandler _handler;
        private readonly Mock<IValidator<UpdateDeliveryUserCommand>> _validator;

        public UpdateUserCommandHandlerTests()
        {
            _deliveryUserServiceMock = new Mock<IDeliveryUserService>();
            _repositoryMock = new Mock<IDeliveryUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _validator = new Mock<IValidator<UpdateDeliveryUserCommand>>();
            _handler = new UpdateDeliveryUserCommandHandler(
                _mapperMock.Object,
                _deliveryUserServiceMock.Object,
                _repositoryMock.Object,
                _validator.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundResult()
        {
            // Arrange
            var command = new UpdateDeliveryUserCommand { Id = 1 };
            _repositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DeliveryUser)null);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Delivery person not found", result.Message);
        }

        [Fact]
        public async Task Handle_UpdatingLicenseType_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UpdateDeliveryUserCommand
            {
                Id = 1,
                LicenseType = LicenseType.A,
                LicenseNumber = "123456",
                LicenseImage = null
            };
            var user = new DeliveryUser { Id = 1, LicenseType = LicenseType.B, LicenseNumber = "654321" };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<DeliveryUser>(command))
                       .Returns(user);
            _deliveryUserServiceMock.Setup(service => service.UpdateUserLicenseImageAsync(user, command.LicenseImage, It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(user);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("License image was updated successfully.", result.Message);
        }

        [Fact]
        public async Task Handle_UpdatingLicenseNumber_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UpdateDeliveryUserCommand
            {
                Id = 1,
                LicenseType = LicenseType.A,
                LicenseNumber = "123456",
                LicenseImage = null
            };
            var user = new DeliveryUser { Id = 1, LicenseType = LicenseType.A, LicenseNumber = "654321" };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<DeliveryUser>(command))
                       .Returns(user);
            _deliveryUserServiceMock.Setup(service => service.UpdateUserLicenseImageAsync(user, command.LicenseImage, It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(user);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("License image was updated successfully.", result.Message);
        }

        [Fact]
        public async Task Handle_NoUpdatesNeeded_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UpdateDeliveryUserCommand
            {
                Id = 1,
                LicenseType = LicenseType.A,
                LicenseNumber = "123456",
                LicenseImage = null
            };
            var user = new DeliveryUser { Id = 1, LicenseType = LicenseType.A, LicenseNumber = "123456" };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<DeliveryUser>(command))
                       .Returns(user);
            _deliveryUserServiceMock.Setup(service => service.UpdateUserLicenseImageAsync(user, command.LicenseImage, It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(user);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("License image was updated successfully.", result.Message);
        }
    }
}
