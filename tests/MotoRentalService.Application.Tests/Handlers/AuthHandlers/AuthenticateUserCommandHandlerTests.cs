using Moq;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Application.MediatR.CommandHandlers.AuthHandlers;
using MotoRentalService.Application.MediatR.Commands.Auth;
using FluentValidation;
using FluentValidation.Results;

namespace MotoRentalService.Application.Tests.Handlers.AuthHandlers
{
    public class AuthenticateUserCommandHandlerTests
    {
        private readonly Mock<IUserAuthenticationService> _userAuthenticationServiceMock;
        private readonly AuthenticateUserCommandHandler _handler;
        private readonly Mock<IValidator<LoginAuthUserCommand>> _validator;

        public AuthenticateUserCommandHandlerTests()
        {
            _userAuthenticationServiceMock = new Mock<IUserAuthenticationService>();
            _validator = new Mock<IValidator<LoginAuthUserCommand>>();
            _handler = new AuthenticateUserCommandHandler(_userAuthenticationServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_SuccessfulAuthentication_ReturnsAuthCommandResult()
        {
            // Arrange
            var command = new LoginAuthUserCommand
            {
                Email = "test@example.com",
                Password = "password123"
            };
            var token = "fake-jwt-token";
            _userAuthenticationServiceMock.Setup(x => x.AuthenticateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                                          .ReturnsAsync(token);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("User is authenticated successfully.", result.Message);
            Assert.Equal(token, result.Token);
        }

        [Fact]
        public async Task Handle_UnsuccessfulAuthentication_ReturnsAuthCommandResultWithFailure()
        {
            // Arrange
            var command = new LoginAuthUserCommand
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };
            _userAuthenticationServiceMock.Setup(x => x.AuthenticateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                                          .ReturnsAsync((string)null);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("user is not valid", result.Message);
            Assert.Null(result.Token);
        }

        [Fact]
        public async Task Handle_UnsuccessfulAuthentication_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var command = new LoginAuthUserCommand
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };
            _userAuthenticationServiceMock.Setup(x => x.AuthenticateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                                          .ThrowsAsync(new UnauthorizedAccessException("Authentication failed"));
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }

        [Fact]
        public async Task Handle_ThrowsException_ReturnsAuthCommandResultWithError()
        {
            // Arrange
            var command = new LoginAuthUserCommand
            {
                Email = "test@example.com",
                Password = "password123"
            };
            _userAuthenticationServiceMock.Setup(x => x.AuthenticateUserAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
                                          .ThrowsAsync(new Exception("Some error"));
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}