using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.MediatR.CommandHandlers.UserHandlers;
using MotoRentalService.Application.MediatR.Commands.User;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.Tests.Handlers.UserHandlers
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly Mock<IDeliveryUserService> _deliveryUserServiceMock;
        private readonly Mock<IUserAuthenticationService> _userAuthenticationServiceMock;
        private readonly Mock<IRentalService> _rentalServiceMock;
        private readonly DeleteDeliveryUserCommandHandler _handler;
        private readonly Mock<IValidator<DeleteDeliveryUserCommand>> _validator;

        public DeleteUserCommandHandlerTests()
        {
            _deliveryUserServiceMock = new Mock<IDeliveryUserService>();
            _userAuthenticationServiceMock = new Mock<IUserAuthenticationService>();
            _rentalServiceMock = new Mock<IRentalService>();
            _validator = new Mock<IValidator<DeleteDeliveryUserCommand>>();
            _handler = new DeleteDeliveryUserCommandHandler(
                _deliveryUserServiceMock.Object,
                _userAuthenticationServiceMock.Object,
                _rentalServiceMock.Object,
                _validator.Object);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsUserNotFoundResult()
        {
            // Arrange
            var command = new DeleteDeliveryUserCommand { Id = 1 };
            _deliveryUserServiceMock.Setup(service => service.GetUserByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                                    .ReturnsAsync((DeliveryUser)null);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User was not found.", result.Message);
        }

        [Fact]
        public async Task Handle_UserHasActiveRental_ReturnsActiveRentalResult()
        {
            // Arrange
            var command = new DeleteDeliveryUserCommand { Id = 1 };
            var user = new DeliveryUser
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                CNPJ = "1231231231231",
                DateOfBirth = DateTime.UtcNow.AddDays(-30),
                Id = command.Id,
                LicenseImageFullName = "test.png",
                LicenseImageURL = "path/test.png",
                LicenseNumber = "123123123",
                LicenseType = LicenseType.B,
                Name = "Test User",
                Status = UserStatus.Approved,
            };
            var rental = new Rental
            {
                Status = RentalStatus.Active,
                Id = 13,
                CreatedAt = DateTime.UtcNow,
                DailyRate = 30,
                ExpectedEndDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(7),
                MotorcycleId = 1,
                StartDate = DateTime.UtcNow,
                RentalPlanId = 1,
                UpdatedAt = DateTime.UtcNow,
                UserId = user.Id,
            };

            _deliveryUserServiceMock.Setup(service => service.GetUserByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(user);
            _rentalServiceMock.Setup(service => service.GetRentByUserIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(rental);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Delivery user has an active rental contract.", result.Message);
        }

        [Fact]
        public async Task Handle_SuccessfulDeletion_ReturnsSuccessResult()
        {
            // Arrange
            var command = new DeleteDeliveryUserCommand { Id = 1 };
            var user = new DeliveryUser { UserId = 14, Id = command.Id };

            _deliveryUserServiceMock.Setup(service => service.GetUserByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(user);
            _rentalServiceMock.Setup(service => service.GetRentByUserIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync((Rental)null);
            _deliveryUserServiceMock.Setup(service => service.DeleteUserAsync(command.Id, It.IsAny<CancellationToken>()))
                                    .Returns(Task.CompletedTask);
            _userAuthenticationServiceMock.Setup(service => service.DeleteUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                                          .Returns(Task.CompletedTask);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Delivery user was deleted successfully.", result.Message);
        }

        [Fact]
        public async Task Handle_UserHasInactiveRental_DeletesRentalAndReturnsSuccessResult()
        {
            // Arrange
            var command = new DeleteDeliveryUserCommand { Id = 1 };
            var user = new DeliveryUser { UserId = 1, Id = command.Id };
            var rental = new Rental { Status = RentalStatus.Completed };

            _deliveryUserServiceMock.Setup(service => service.GetUserByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                                    .ReturnsAsync(user);
            _rentalServiceMock.Setup(service => service.GetRentByUserIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(rental);
            _rentalServiceMock.Setup(service => service.DeleteRentalByUserIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                              .Returns(Task.CompletedTask);
            _deliveryUserServiceMock.Setup(service => service.DeleteUserAsync(command.Id, It.IsAny<CancellationToken>()))
                                    .Returns(Task.CompletedTask);
            _userAuthenticationServiceMock.Setup(service => service.DeleteUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                                          .Returns(Task.CompletedTask);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Delivery user was deleted successfully.", result.Message);
        }
    }
}
