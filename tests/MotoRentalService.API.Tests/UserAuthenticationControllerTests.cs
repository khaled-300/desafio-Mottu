using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotoRentalService.API.Controllers;
using MotoRentalService.Application.MediatR.Commands.Auth;
using MotoRentalService.Application.MediatR.Response.Auth;
using MotoRentalService.Application.Request.Auth;

namespace MotoRentalService.API.Tests
{
    public class UserAuthenticationControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<UserAuthenticationController>> _loggerMock;
        private readonly UserAuthenticationController _controller;

        public UserAuthenticationControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<UserAuthenticationController>>();
            _controller = new UserAuthenticationController(_mediatorMock.Object, _loggerMock.Object);
        }
        
        [Fact]
        public async void CreateUser_WithValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new CreateAuthUserRequest
            {
                Email = "test@outlook.com",
                Password = "123123123",
                Role = Domain.ValueObjects.UserRole.Admin
            };

            var expectedResult = new AuthCommandResult { Success = true, Message = "user created successfully.", Token = "token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterAuthUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateUser(request, CancellationToken.None) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
            result.Value.Should().BeEquivalentTo(expectedResult);
            var commandResult = result.Value as AuthCommandResult;
            commandResult.Should().NotBeNull();
            commandResult.Token.Should().NotBeNullOrEmpty();
            commandResult.Token.Should().Be(expectedResult.Token);
        }

        [Fact]
        public async void CreateUser_WithInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateAuthUserRequest();

            var expectedResult = new AuthCommandResult { Success = false, Message = "Validation failed" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterAuthUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateUser(request, CancellationToken.None) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            result.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task AuthenticateUser_ReturnsOkResult_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginAuthUserRequest
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var commandResult = new AuthCommandResult
            {
                Success = true,
                Token = "Token",
                Message = "authenticate user is successfull."
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginAuthUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Should().NotBeNull();
            Assert.NotNull(okResult.Value);
            var comResult = okResult.Value as AuthCommandResult;
            comResult.Should().NotBeNull();
            comResult.Success.Should().BeTrue();
            comResult.Token.Should().Be(commandResult.Token);
        }

        [Fact]
        public async Task AuthenticateUser_ReturnsBadRequest_WhenCredentialsAreInvalid()
        {
            // Arrange
            var request = new LoginAuthUserRequest
            {
                Email = "user@example.com",
                Password = "password123"
            };

            var commandResult = new AuthCommandResult
            {
                Success = false,
                Message = "Authentication failed."
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginAuthUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.AuthenticateUser(request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Should().NotBeNull();
            Assert.NotNull(badRequestResult.Value);
            Assert.NotNull(badRequestResult.Value);
            var comResult = badRequestResult.Value as AuthCommandResult;
            comResult.Should().NotBeNull();
            comResult.Success.Should().BeFalse();
        }
    }
}
