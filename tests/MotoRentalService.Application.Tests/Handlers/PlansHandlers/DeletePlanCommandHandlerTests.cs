using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MotoRentalService.Application.MediatR.CommandHandlers.PlansHandlers;
using MotoRentalService.Application.MediatR.Commands.Plans;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.Tests.Handlers.PlansHandlers
{
    public class DeletePlanCommandHandlerTests
    {
        private readonly Mock<IPlansRepository> _plansRepositoryMock;
        private readonly DeletePlanCommandHandler _handler;
        private readonly Mock<IValidator<DeletePlanCommand>> _validator;
        public DeletePlanCommandHandlerTests()
        {
            _plansRepositoryMock = new Mock<IPlansRepository>();
            _validator = new Mock<IValidator<DeletePlanCommand>>();
            _handler = new DeletePlanCommandHandler(_plansRepositoryMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_PlanDeletedSuccessfully_ReturnsSuccessResult()
        {
            // Arrange
            var command = new DeletePlanCommand { Id = 1 };

            _plansRepositoryMock.Setup(repo => repo.DeletePlanAsync(command.Id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("The Plan was deleted successfully", result.Message);
        }

        [Fact]
        public async Task Handle_PlanDeletionFailed_ThrowsException()
        {
            // Arrange
            var command = new DeletePlanCommand { Id = 1 };

            _plansRepositoryMock.Setup(repo => repo.DeletePlanAsync(command.Id, It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Plan deletion failed"));
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}
