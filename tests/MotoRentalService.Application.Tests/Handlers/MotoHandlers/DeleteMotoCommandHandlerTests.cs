using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.MediatR.CommandHandlers.MotoHandlers;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.Tests.Handlers.MotoHandlers
{
    public class DeleteMotoCommandHandlerTests
    {
        private readonly Mock<IMotorcycleRepository> _repositoryMock;
        private readonly DeleteMotoCommandHandler _handler;
        private readonly Mock<IValidator<DeleteMotoCommand>> _validator;

        public DeleteMotoCommandHandlerTests()
        {
            _repositoryMock = new Mock<IMotorcycleRepository>();
            _validator = new Mock<IValidator<DeleteMotoCommand>>();
            _handler = new DeleteMotoCommandHandler(_repositoryMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_MotorcycleFoundAndDeleted_ReturnsSuccessResult()
        {
            // Arrange
            var command = new DeleteMotoCommand { Id = 1 };
            var motorcycle = new Motorcycle { Id = 1, LicensePlate = "ABC123", Model = "Test Model", Year = 2022 };

            _repositoryMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(motorcycle);
            _repositoryMock.Setup(r => r.DeleteAsync(motorcycle, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Motorcycle deleted successfully!", result.Message);
        }

        [Fact]
        public async Task Handle_MotorcycleNotFound_ReturnsFailureResult()
        {
            // Arrange
            var command = new DeleteMotoCommand { Id = 1 };

            _repositoryMock.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Motorcycle)null);
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
