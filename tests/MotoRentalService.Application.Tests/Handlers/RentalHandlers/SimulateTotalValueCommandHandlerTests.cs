using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.MediatR.CommandHandlers.RentalHandlers;
using MotoRentalService.Application.MediatR.Commands.Rental;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.Tests.Handlers.RentalHandlers
{
    public class SimulateTotalValueCommandHandlerTests
    {
        private readonly Mock<IRentalService> _rentalServiceMock;
        private readonly SimulateTotalValueCommandHandler _handler;
        private readonly Mock<IValidator<SimulateTotalValueCommand>> _validator;

        public SimulateTotalValueCommandHandlerTests()
        {
            _rentalServiceMock = new Mock<IRentalService>();
            _validator = new Mock<IValidator<SimulateTotalValueCommand>>();
            _handler = new SimulateTotalValueCommandHandler(_rentalServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_SimulationSuccessful_ReturnsCorrectValue()
        {
            // Arrange
            var command = new SimulateTotalValueCommand
            {
                Id = 1,
                ReturnDate = DateTime.UtcNow.AddDays(5)
            };
            var simulationValue = 150.0m;

            var rental = new Rental { Id = 1 };

            _rentalServiceMock.Setup(service => service.GetRentByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(rental);
            _rentalServiceMock.Setup(service => service.CalculateFinalPriceAsync(rental, command.ReturnDate, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(simulationValue);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(simulationValue, result.TotalValue);
            Assert.Contains("was executed successfully", result.Message);
        }

        [Fact]
        public async Task Handle_RentalNotFound_ThrowsArgumentNullException()
        {
            // Arrange
            var command = new SimulateTotalValueCommand { Id = 99 };

            _rentalServiceMock.Setup(service => service.GetRentByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                              .ReturnsAsync((Rental)null);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}
