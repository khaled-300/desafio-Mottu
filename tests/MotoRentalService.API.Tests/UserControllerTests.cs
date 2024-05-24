using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotoRentalService.API.Controllers;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.User;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.Response.DeliveryUser;
using MotoRentalService.Application.Request.DeliveryUser;
using System.Security.Claims;
using System.Text;

namespace MotoRentalService.API.Tests
{
    public class DeliveryUsersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<DeliveryUsersController>> _loggerMock;
        private readonly DeliveryUsersController _controller;
        
        public DeliveryUsersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<DeliveryUsersController>>();
            _controller = new DeliveryUsersController(_mediatorMock.Object, _loggerMock.Object)
            {
                // Set the mock user context
                ControllerContext = MockHttpContextWithUser()
            };
        }

        /// <summary>
        /// Helper function to create a mock HttpContext with ClaimsPrincipal
        /// </summary>
        /// <returns></returns>
        private ControllerContext MockHttpContextWithUser(int userId = 1)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, "test@example.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(ctx => ctx.User).Returns(claimsPrincipal);

            return new ControllerContext
            {
                HttpContext = contextMock.Object
            };
        }

        [Fact]
        public async void GetUserId_WithValidId_ReturnsUserDto()
        {
            // Arrange
            int userId = 1;
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "licenseImage", "licenseImage.png");
            var expectedUserDto = new DeliveryUserDto
            {
                Id = userId,
                CNPJ = "11111111111111",
                DateOfBirth = new DateTime(),
                CreatedAt = new DateTime(),
                LicenseNumber = "123123123123",
                LicenseImageURL = "test",
                LicenseType = Domain.ValueObjects.LicenseType.None,
                Name = "Test",
                LicenseImage = file,
                UpdatedAt = null
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUserDto);

            // Act
            var result = await _controller.GetUserId(userId, CancellationToken.None) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);

            var userDto = result.Value as DeliveryUserDto;
            userDto.Should().NotBeNull();
            userDto.Should().BeEquivalentTo(expectedUserDto);
        }

        [Fact]
        public async void GetUserId_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            int userId = 1;

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DeliveryUserDto)null);

            // Act
            var result = await _controller.GetUserId(userId, CancellationToken.None) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async void GetUserId_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            int userId = 1;

            var mediatorMock = new Mock<IMediator>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetUserId(userId, CancellationToken.None) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async void CreateUser_WithValidRequest_ReturnsCreated()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "licenseImage", "licenseImage.png");
            var request = new CreateDeliveryUserRequest
            {
                CNPJ = "12345678901234",
                DateOfBirth = DateTime.Now.Date,
                LicenseImage = file,
                LicenseNumber = "ABC123",
                LicenseType = Domain.ValueObjects.LicenseType.None,
                Name = "John Doe"
            };

            var expectedUserDto = new DeliveryUserDto { /* Fill with expected user data */ };
            var expectedResult = new DeliveryUserCommandResult { Success = true, DeliveryUser = expectedUserDto };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserDeliveryCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateUser(request, CancellationToken.None) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status201Created);

            var createdUser = result.Value as DeliveryUserCommandResult;
            createdUser.Should().NotBeNull();
            createdUser.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void CreateUser_WithInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateDeliveryUserRequest();

            var expectedResult = new DeliveryUserCommandResult { Success = false, Message = "Validation failed" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserDeliveryCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.CreateUser(request, CancellationToken.None) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var badRequestMessage = result.Value as DeliveryUserCommandResult;
            badRequestMessage.Should().NotBeNull();
            badRequestMessage.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async void CreateUser_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var request = new CreateDeliveryUserRequest();

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserDeliveryCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateUser(request, CancellationToken.None) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidRequest_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "licenseImage", "licenseImage.png");
            var request = new UpdateDeliveryUserRequest
            {
                LicenseType = Domain.ValueObjects.LicenseType.A,
                LicenseImage = file,
                LicenseNumber = "123123123123"
            };
            var expectedResult = new DeliveryUserCommandResult(new DeliveryUserDto
            {
                CNPJ = "111111111111111",
                CreatedAt = DateTime.UtcNow,
                DateOfBirth = DateTime.UtcNow,
                Id = userId,
                LicenseImageURL = "test",
                Name =  "Test",
                UpdatedAt = DateTime.UtcNow,
                LicenseType = Domain.ValueObjects.LicenseType.A,
                LicenseImage = file,
                LicenseNumber = "123123123123"
            }, true, "Success");

            _mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<DeliveryUserCommandResult>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            _controller.ControllerContext = MockHttpContextWithUser(userId);
          
            // Act
            var result = await _controller.UpdateUserAsync(userId, request, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
            var commandResult = okResult.Value as DeliveryUserCommandResult;
            Assert.NotNull(commandResult);
            Assert.Equal(expectedResult.DeliveryUser, commandResult.DeliveryUser);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ReturnsOk()
        {
            // Arrange
            int userId = 1;
            var expectedResult = new DeliveryUserCommandResult(null, true, "Success");
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDeliveryUserCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(expectedResult);


            // Act
            var result = await _controller.DeleteAsync(userId, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
            Assert.Equal(expectedResult, okResult.Value as DeliveryUserCommandResult);
            var commandResult = okResult.Value as DeliveryUserCommandResult;
            Assert.NotNull(commandResult);
            Assert.Equal(expectedResult.Success, commandResult.Success);
        }
    }
}