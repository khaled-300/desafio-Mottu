using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MotoRentalService.API.Controllers;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.Response.Plans;
using MotoRentalService.Application.Request.Plans;

namespace MotoRentalService.API.Tests
{
    public class PlanControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<PlansController>> _loggerMock;
        private readonly PlansController _controller;

        public PlanControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<PlansController>>();
            _controller = new PlansController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetPlanById_ReturnsOkWithPlan_WhenPlanExists()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var planId = 1;
            var planDto = new PlanDto
            {
                Id = planId,
                Name = "Basic Plan",
                CreatedAt = DateTime.UtcNow,
                DailyRate = 10M,
                DurationInDays = 7,
                IsActive = true,
                UpdatedAt = DateTime.UtcNow
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPlanByIdQuery>(), cancellationToken))
                         .ReturnsAsync(planDto);

            // Act
            var result = await _controller.GetPlanById(planId, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returnedPlan = Assert.IsType<PlanDto>(okResult.Value);
            returnedPlan.Should().BeEquivalentTo(planDto);
        }

        [Fact]
        public async Task GetPlanById_ReturnsNotFound_WhenPlanDoesNotExist()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var planId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPlanByIdQuery>(), cancellationToken))
                         .ReturnsAsync(value: null);

            // Act
            var result = await _controller.GetPlanById(planId, cancellationToken);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().Be($"No rental plan found with ID: {planId}");
        }

        [Fact]
        public async Task GetPlanById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var planId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPlanByIdQuery>(), cancellationToken))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetPlanById(planId, cancellationToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }


        [Fact]
        public async Task GetPlansAsync_ReturnsOkWithPlans_WhenPlansExist()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var plans = new List<PlanDto>
            {
                new PlanDto { 
                    Id = 1,
                    Name = "15 days Plan",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    DurationInDays = 15,
                    DailyRate = 60M 
                },
                new PlanDto { 
                    Id = 2, 
                    Name = "7 days Plan", 
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow, 
                    IsActive = true, 
                    DurationInDays = 7, 
                    DailyRate = 30M 
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPlansQuery>(), cancellationToken))
                         .ReturnsAsync(plans);

            // Act
            var result = await _controller.GetPlansAsync(cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPlans = Assert.IsType<List<PlanDto>>(okResult.Value);
            returnedPlans.Should().BeEquivalentTo(plans);
        }

        [Fact]
        public async Task GetPlansAsync_ReturnsNotFound_WhenNoPlansAreFound()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var plans = new List<PlanDto>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPlansQuery>(), cancellationToken))
                         .ReturnsAsync(plans);

            // Act
            var result = await _controller.GetPlansAsync(cancellationToken);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().Be("No rental plans were found.");
        }

        [Fact]
        public async Task GetPlansAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPlansQuery>(), cancellationToken))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetPlansAsync(cancellationToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(500);
            statusCodeResult.Value.Should().Be("An error occurred while processing your request.");
        }


        [Fact]
        public async Task CreatePlanAsync_ReturnsCreatedAtAction_WhenCreationIsSuccessful()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new CreatePlanRequest
            {
                DailyRate = 100,
                DurationInDays = 7,
                IsActive = true,
                Name = "New Plan"
            };

            var commandResult = new PlansCommandResult
            {
                Success = true,
                Plan = new PlanDto { 
                    Id = 1, 
                    Name = "New Plan",
                    DailyRate = 100,
                    DurationInDays= 7,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePlanCommand>(), cancellationToken))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreatePlan(request, cancellationToken);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            createdAtActionResult.ActionName.Should().Be("GetPlanById");
            createdAtActionResult.RouteValues["id"].Should().Be(1);
            createdAtActionResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task CreatePlanAsync_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new CreatePlanRequest
            {
                DailyRate = 100,
                DurationInDays = 7,
                IsActive = true,
                Name = "Faulty Plan"
            };
            var commandResult = new PlansCommandResult
            {
                Success = false,
                Message = "Invalid data provided."
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePlanCommand>(), cancellationToken))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreatePlan(request, cancellationToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreatePlanAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new CreatePlanRequest();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePlanCommand>(), cancellationToken))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreatePlan(request, cancellationToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task UpdatePlanAsync_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new UpdatePlanRequest
            {
                Name = "Updated Plan",
                IsActive = true,
                DailyRate = 120,
                DurationInDays = 10
            };
            var commandResult = new PlansCommandResult
            {
                Success = true,
                Plan = new PlanDto { Id = 1, Name = "Updated Plan" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePlanCommand>(), cancellationToken))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.UpdatePlanAsync(1, request, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(commandResult);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task UpdatePlanAsync_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new UpdatePlanRequest
            {
                Name = "Faulty Plan",
                IsActive = false,
                DailyRate = 100,
                DurationInDays = 5
            };
            var commandResult = new PlansCommandResult
            {
                Success = false,
                Message = "Validation failed."
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePlanCommand>(), cancellationToken))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.UpdatePlanAsync(1, request, cancellationToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().BeEquivalentTo(commandResult);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task UpdatePlanAsync_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var request = new UpdatePlanRequest();
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdatePlanCommand>(), cancellationToken))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdatePlanAsync(1, request, cancellationToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task DeletePlanAsync_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var commandResult = new PlansCommandResult
            {
                Success = true,
                Plan = new PlanDto
                {
                    Id = 1,
                    Name = "New Plan",
                    DailyRate = 100,
                    DurationInDays = 7,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeletePlanCommand>(), cancellationToken))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.DeletePlanAsync(1, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(commandResult);
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task DeletePlanAsync_ReturnsBadRequest_WhenDeletionFails()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var commandResult = new PlansCommandResult
            {
                Success = false,
                Message = "Deletion failed due to dependency."
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeletePlanCommand>(), cancellationToken))
                         .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.DeletePlanAsync(1, cancellationToken);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().BeEquivalentTo(commandResult);
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task DeletePlanAsync_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeletePlanCommand>(), cancellationToken))
                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.DeletePlanAsync(1, cancellationToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
