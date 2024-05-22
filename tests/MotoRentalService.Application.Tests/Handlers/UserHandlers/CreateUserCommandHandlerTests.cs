using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.CommandHandlers.UserHandlers;
using MotoRentalService.Application.MediatR.Commands.User;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.Tests.Handlers.UserHandlers
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IDeliveryUserService> _deliveryPersonServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateDeliveryUserCommandHandler _handler;
        private readonly Mock<IValidator<CreateUserDeliveryCommand>> _validator;

        public CreateUserCommandHandlerTests()
        {
            _deliveryPersonServiceMock = new Mock<IDeliveryUserService>();
            _mapperMock = new Mock<IMapper>();
            _validator = new Mock<IValidator<CreateUserDeliveryCommand>>();
            _handler = new CreateDeliveryUserCommandHandler(_mapperMock.Object, _deliveryPersonServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_UserCreationSuccessful_ReturnsCorrectResponse()
        {
            // Arrange
            var command = new CreateUserDeliveryCommand
            {
                AuthUserId = 1,
                Name = "John Doe",
                CNPJ = "123456789",
                DateOfBirth = DateTime.Now.AddYears(-30),
                LicenseNumber = "123456",
                LicenseType = LicenseType.None,
                LicenseImage = null,
            };
            var deliveryUser = new DeliveryUser
            {
                UserId = command.AuthUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                CNPJ = command.CNPJ,
                DateOfBirth = command.DateOfBirth,
                Id = 1,
                LicenseImageFullName = "test.png",
                LicenseImageURL = "path/test.png",
                LicenseNumber = command.LicenseNumber,
                LicenseType = command.LicenseType,
                Name = command.Name,
                Status = UserStatus.Pending,
                User = new Users
                {
                    CreatedAt = DateTime.UtcNow,
                    Email = "test@gmail.com",
                    Id = 1,
                    Password = "asjdhawdha!@$#ASDCASD",
                    Role = UserRole.Admin,
                    UpdatedAt = null
                }
            };
            var deliveryUserDto = new DeliveryUserDto
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                CNPJ = command.CNPJ,
                DateOfBirth = command.DateOfBirth,
                LicenseImageURL = "path/test.png",
                LicenseNumber = command.LicenseNumber,
                LicenseType = command.LicenseType,
                Name = command.Name,
            };

            _mapperMock.Setup(mapper => mapper.Map<DeliveryUser>(command)).Returns(deliveryUser);
            _mapperMock.Setup(mapper => mapper.Map<DeliveryUserDto>(deliveryUser)).Returns(deliveryUserDto);
            _deliveryPersonServiceMock.Setup(service => service.GetUserByUserIdAsync(deliveryUser.UserId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync((DeliveryUser)null);
            _deliveryPersonServiceMock.Setup(service => service.RegisterUserAsync(deliveryUser, command.LicenseImage, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(deliveryUser);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.DeliveryUser);
            Assert.Equal("Delivery user was created succsessfully.", result.Message);
        }

        [Fact]
        public async Task Handle_UserAlreadyExists_ReturnsCorrectResponse()
        {
            // Arrange
            var command = new CreateUserDeliveryCommand
            {
                AuthUserId = 1,
                Name = "John Doe",
                CNPJ = "123456789",
                DateOfBirth = DateTime.Now.AddYears(-30),
                LicenseNumber = "123456",
                LicenseType = LicenseType.None,
                LicenseImage = null,
            };
            var existingUser = new DeliveryUser
            {
                UserId = command.AuthUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                CNPJ = command.CNPJ,
                DateOfBirth = command.DateOfBirth,
                Id = 1,
                LicenseImageFullName = "test.png",
                LicenseImageURL = "path/test.png",
                LicenseNumber = command.LicenseNumber,
                LicenseType = command.LicenseType,
                Name = command.Name,
                Status = UserStatus.Pending,
            };

            _mapperMock.Setup(mapper => mapper.Map<DeliveryUser>(command)).Returns(existingUser);

            _deliveryPersonServiceMock.Setup(service => service.GetUserByUserIdAsync(command.AuthUserId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(existingUser);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Null(result.DeliveryUser);
            Assert.Equal("user has a record in the database please use the update or get endpoints.", result.Message);
        }
    }
}
