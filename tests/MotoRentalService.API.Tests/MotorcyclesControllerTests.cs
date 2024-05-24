using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotoRentalService.API.Controllers;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.Response.Moto;
using MotoRentalService.Application.Request.Moto;

namespace MotoRentalService.API.Tests
{
    public class MotorcyclesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<MotorcyclesController>> _loggerMock;
        private readonly MotorcyclesController _controller;

        public MotorcyclesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<MotorcyclesController>>();
            _controller = new MotorcyclesController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateMotorcycleAsync_ReturnsCreatedAtAction_WhenCreationIsSuccessful()
        {
            // Arrange
            var request = new CreateMotorcycleRequest
            {
                LicensePlate = "XYZ123",
                Model = "Honda CBR",
                Year = 2021
            };
            var commandResult = new MotorcycleCommandResult
            {
                Success = true,
                Motorcycle = new MotorcycleDto
                {
                    Id = 1,
                    LicensePlate = "XYZ-1231",
                    Model = "Honda CBR",
                    Year = 2021,
                    IsRented = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreateMotorcycleAsync(request, CancellationToken.None);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            createdAtActionResult.StatusCode.Should().Be(StatusCodes.Status201Created);
            createdAtActionResult.Value.Should().BeEquivalentTo(commandResult.Motorcycle);
            createdAtActionResult.ActionName.Should().Be("GetMotorcycle");
            createdAtActionResult.RouteValues["motorcycleId"].Should().Be(1);
        }

        [Fact]
        public async Task CreateMotorcycleAsync_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var request = new CreateMotorcycleRequest
            {
                LicensePlate = "XYZ123",
                Model = "Honda CBR",
                Year = 2021
            };
            var commandResult = new MotorcycleCommandResult
            {
                Success = false,
                Message = "Invalid data provided."
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreateMotorcycleAsync(request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateMotorcycleAsync_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new CreateMotorcycleRequest
            {
                LicensePlate = "XYZ123",
                Model = "Honda CBR",
                Year = 2021
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateMotorcycleAsync(request, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetMotorcyclesAsync_ReturnsOk_WithMotorcycles()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var motorcycleList = new List<MotorcycleDto>
        {
            new MotorcycleDto { Id = 1, LicensePlate = "XYZ123", Model = "Honda CBR", Year = 2021 },
            new MotorcycleDto { Id = 2, LicensePlate = "ABC789", Model = "Yamaha R6", Year = 2022 }
        };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMotoPageQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(motorcycleList);

            // Act
            var result = await _controller.GetMotorcyclesAsync(pageNumber, pageSize, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(motorcycleList);
        }

        [Fact]
        public async Task GetMotorcyclesAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMotoPageQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetMotorcyclesAsync(pageNumber, pageSize, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }

        [Fact]
        public async Task GetMotorcycle_ReturnsOk_WithMotorcycle()
        {
            // Arrange
            var motorcycleId = 1;
            var motorcycle = new MotorcycleDto
            {
                Id = motorcycleId,
                LicensePlate = "XYZ123",
                Model = "Honda CBR",
                Year = 2021,
                CreatedAt = DateTime.UtcNow,
                IsRented = true,
                UpdatedAt = DateTime.UtcNow
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMotoByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(motorcycle);

            // Act
            var result = await _controller.GetMotorcycle(motorcycleId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(motorcycle);
        }

        [Fact]
        public async Task GetMotorcycle_ReturnsNotFound_WhenMotorcycleNotFound()
        {
            // Arrange
            var motorcycleId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMotoByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((MotorcycleDto)null);

            // Act
            var result = await _controller.GetMotorcycle(motorcycleId, CancellationToken.None);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().Be($"No motorcycle found with ID: {motorcycleId}");
        }

        [Fact]
        public async Task GetMotorcycle_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var motorcycleId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMotoByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetMotorcycle(motorcycleId, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }

        [Fact]
        public async Task UpdateMotorcycleAsync_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var motorcycleId = 1;
            var licensePlate = "XYZ123";
            var commandResult = new MotorcycleCommandResult
            {
                Success = true,
                Message = "Motorcycle updated.",
                Motorcycle = new MotorcycleDto
                {
                    Id = motorcycleId,
                    LicensePlate = licensePlate,
                    UpdatedAt = DateTime.UtcNow,
                    IsRented = true,
                    CreatedAt = DateTime.UtcNow,
                    Model = "123123",
                    Year = 2012
                }
            };

            var command = new UpdateMotoCommand
            {
                LicensePlate = licensePlate
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.UpdateMotorcycleAsync(motorcycleId, command, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(commandResult);
        }

        [Fact]
        public async Task UpdateMotorcycleAsync_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var motorcycleId = 1;
            var licensePlate = "XYZ123";
            var commandResult = new MotorcycleCommandResult
            {
                Success = false,
                Message = "Invalid license plate."
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);
            var command = new UpdateMotoCommand
            {
                LicensePlate = licensePlate
            };

            // Act
            var result = await _controller.UpdateMotorcycleAsync(motorcycleId, command, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().BeEquivalentTo(commandResult);
        }

        [Fact]
        public async Task UpdateMotorcycleAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var motorcycleId = 1;
            var licensePlate = "XYZ123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));
            var command = new UpdateMotoCommand
            {
                LicensePlate = licensePlate
            };
            // Act
            var result = await _controller.UpdateMotorcycleAsync(motorcycleId, command, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }

        [Fact]
        public async Task DeleteMotorcycleAsync_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var motorcycleId = 1;
            var commandResult = new MotorcycleCommandResult { Success = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.DeleteMotorcycleAsync(motorcycleId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(commandResult);
        }

        [Fact]
        public async Task DeleteMotorcycleAsync_ReturnsBadRequest_WhenDeletionFails()
        {
            // Arrange
            var motorcycleId = 1;
            var commandResult = new MotorcycleCommandResult
            {
                Success = false,
                Message = "Deletion failed due to dependency."
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.DeleteMotorcycleAsync(motorcycleId, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().BeEquivalentTo(commandResult);
        }

        [Fact]
        public async Task DeleteMotorcycleAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var motorcycleId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteMotoCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.DeleteMotorcycleAsync(motorcycleId, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }
    }
}
