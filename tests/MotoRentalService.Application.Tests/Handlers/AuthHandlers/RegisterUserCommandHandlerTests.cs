using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.MediatR.CommandHandlers.AuthHandlers;
using MotoRentalService.Application.MediatR.Commands.Auth;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.Tests.Handlers.AuthHandlers
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserAuthenticationService> _userAuthenticationServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RegisterUserCommandHandler _handler;
        private readonly Mock<IValidator<RegisterAuthUserCommand>> _validator;

        public RegisterUserCommandHandlerTests()
        {
            _userAuthenticationServiceMock = new Mock<IUserAuthenticationService>();
            _mapperMock = new Mock<IMapper>();
            _validator = new Mock<IValidator<RegisterAuthUserCommand>>();
            _handler = new RegisterUserCommandHandler(_mapperMock.Object, _userAuthenticationServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_SuccessfulRegistration_ReturnsAuthCommandResult()
        {
            // Arrange
            var command = new RegisterAuthUserCommand
            {
                Email = "test@example.com",
                Password = "password",
                Role = Domain.ValueObjects.UserRole.User
            };

            var user = new Users
            {
                Email = command.Email,
                Password = command.Password,
                Role = command.Role
            };

            _mapperMock.Setup(m => m.Map<Users>(command)).Returns(user);
            _userAuthenticationServiceMock.Setup(u => u.RegisterUserAsync(user, It.IsAny<CancellationToken>()))
                                          .ReturnsAsync("sample_token");
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("sample_token", result.Token);
            Assert.Equal("User has been created successfully.", result.Message);
        }

        [Fact]
        public async Task Handle_UnsuccessfulRegistration_ReturnsFailedAuthCommandResult()
        {
            // Arrange
            var command = new RegisterAuthUserCommand
            {
                Email = "test@example.com",
                Password = "password",
                Role = Domain.ValueObjects.UserRole.Admin
            };

            var user = new Users
            {
                Email = command.Email,
                Password = command.Password,
                Role = command.Role
            };

            _mapperMock.Setup(m => m.Map<Users>(command)).Returns(user);
            _userAuthenticationServiceMock.Setup(u => u.RegisterUserAsync(user, It.IsAny<CancellationToken>()))
                                          .ReturnsAsync(string.Empty);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}
