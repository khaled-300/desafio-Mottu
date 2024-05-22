using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotoRentalService.API.Controllers;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Rental;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.Response.Rental;
using MotoRentalService.Application.Request.Rental;

namespace MotoRentalService.API.Tests
{
    public class RentalControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<RentalController>> _loggerMock;
        private readonly RentalController _controller;

        public RentalControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<RentalController>>();
            _controller = new RentalController(_mediatorMock.Object, _loggerMock.Object);
        }


        [Fact]
        public async Task CreateRentalAsync_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var request = new CreateRentalRequest
            {
                StartDate = DateTime.Today.ToUniversalTime(),
                EndDate = DateTime.Today.AddDays(5).ToUniversalTime(),
                MotorcycleId = 1,
                RentalPlanId = 1,
                UserId = 3
            };

            var commandResult = new RentalCommandResult
            {
                Success = true,
                Rental = new RentalDto
                {
                    Id = 10,
                    CreatedAt = DateTime.Today.ToUniversalTime(),
                    DailyRate = 1.10M,
                    EndDate = DateTime.Today.AddDays(7).ToUniversalTime(),
                    ExpectedEndDate = DateTime.Today.AddDays(7).ToUniversalTime(),
                    MotorcycleId = 1,
                    RentalPlanId = 1,
                    StartDate = DateTime.UtcNow,
                    Status = Domain.ValueObjects.RentalStatus.Pending,
                    TotalPrice = 0,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = 3
                },
                Message = "Rental contract is created"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRentalCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreateRentalAsync(request, CancellationToken.None);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            createdAtActionResult.Value.Should().BeEquivalentTo(commandResult);
            createdAtActionResult.ActionName.Should().Be("GetRentalById");
            createdAtActionResult.RouteValues["rentalId"].Should().Be(10);
        }

        [Fact]
        public async Task CreateRentalAsync_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var request = new CreateRentalRequest
            {
                StartDate = DateTime.Today.ToUniversalTime(),
                RentalPlanId = 1,
                UserId = 0,
                EndDate = DateTime.Today.AddDays(7).ToUniversalTime(),
                MotorcycleId = 1
            };
            var commandResult = new RentalCommandResult
            {
                Success = false,
                Message = "Invalid parameters"
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRentalCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreateRentalAsync(request, CancellationToken.None);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().BeEquivalentTo(commandResult);
        }

        [Fact]
        public async Task CreateRentalAsync_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var request = new CreateRentalRequest();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRentalCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.CreateRentalAsync(request, CancellationToken.None);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(500);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }


        [Fact]
        public async Task GetRentalById_ReturnsRentalDto_WhenRentalExists()
        {
            // Arrange
            int rentalId = 1;
            var cancellationToken = CancellationToken.None;
            var rentalDto = new RentalDto
            {
                Id = rentalId,
                CreatedAt = DateTime.Today,
                MotorcycleId = 1,
                EndDate = DateTime.Today,
                UserId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.Today,
                DailyRate = 1,
                ExpectedEndDate = DateTime.Today,
                Status = Domain.ValueObjects.RentalStatus.Active,
                TotalPrice = 1,
                UpdatedAt = DateTime.Today
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRentalByIdQuery>(), cancellationToken))
                         .ReturnsAsync(rentalDto);

            // Act
            var result = await _controller.GetRentalById(rentalId, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedDto = Assert.IsType<RentalDto>(okResult.Value);
            returnedDto.Should().BeEquivalentTo(rentalDto);
        }

        [Fact]
        public async Task GetRentalById_ReturnsNotFound_WhenRentalDoesNotExist()
        {
            // Arrange
            int rentalId = 15;
            var cancellationToken = CancellationToken.None;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRentalByIdQuery>(), cancellationToken))
                         .ReturnsAsync((RentalDto)null);

            // Act
            var result = await _controller.GetRentalById(rentalId, cancellationToken);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetRentalById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            int rentalId = 1;
            var cancellationToken = CancellationToken.None;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRentalByIdQuery>(), cancellationToken))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetRentalById(rentalId, cancellationToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }

        [Fact]
        public async Task SimulateReturnDateAsync_ReturnsSimulateTotalValueResult_WhenSuccess()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new SimulateTotalValueRequest { Id = 1, ReturnDate = DateTime.UtcNow };
            var resultDto = new SimulateTotalValueResult(120M, true, "simulation of total value is calculated successfully.");
            _mediatorMock.Setup(m => m.Send(It.IsAny<SimulateTotalValueCommand>(), cancellationToken))
                         .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.SimulateReturnDateAsync(request, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedResult = Assert.IsType<SimulateTotalValueResult>(okResult.Value);
            returnedResult.Should().BeEquivalentTo(resultDto);
            returnedResult.Success.Should().BeTrue();
            returnedResult.TotalValue.Should().Be(resultDto.TotalValue);
        }

        [Fact]
        public async Task SimulateReturnDateAsync_ReturnsBadRequest_WhenNotSuccess()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new SimulateTotalValueRequest { Id = 1, ReturnDate = DateTime.UtcNow };
            var resultDto = new SimulateTotalValueResult { Success = false, Message = "Error trying to caluclate the total value" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SimulateTotalValueCommand>(), cancellationToken))
                             .ReturnsAsync(resultDto);

            // Act
            var result = await _controller.SimulateReturnDateAsync(request, cancellationToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().Be(resultDto);
        }

        [Fact]
        public async Task SimulateReturnDateAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new SimulateTotalValueRequest { Id = 1, ReturnDate = DateTime.UtcNow };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SimulateTotalValueCommand>(), cancellationToken))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.SimulateReturnDateAsync(request, cancellationToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }
    }
}
