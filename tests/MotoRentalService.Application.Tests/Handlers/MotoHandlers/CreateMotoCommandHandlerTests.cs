using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.CommandHandlers.MotoHandlers;
using MotoRentalService.Application.MediatR.Commands.Motorcycle;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.Tests.Handlers.MotoHandlers
{
    public class CreateMotoCommandHandlerTests
    {
        private readonly Mock<IMotorcycleService> _motorcycleServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<CreateMotoCommand>> _validator;
        private readonly CreateMotoCommandHandler _handler;

        public CreateMotoCommandHandlerTests()
        {
            _motorcycleServiceMock = new Mock<IMotorcycleService>();
            _mapperMock = new Mock<IMapper>();
            _validator = new Mock<IValidator<CreateMotoCommand>>();
            _handler = new CreateMotoCommandHandler(_mapperMock.Object, _motorcycleServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_SuccessfulCreation_ReturnsMotorcycleCommandResult()
        {
            // Arrange
            var command = new CreateMotoCommand
            {
                LicensePlate = "ABC123",
                Model = "Test Model",
                Year = 2022
            };

            var motorcycle = new Motorcycle
            {
                LicensePlate = command.LicensePlate,
                Model = command.Model,
                Year = command.Year
            };

            var motorcycleDto = new MotorcycleDto
            {
                LicensePlate = command.LicensePlate,
                Model = command.Model,
                Year = command.Year
            };

            _mapperMock.Setup(m => m.Map<Motorcycle>(command)).Returns(motorcycle);
            _motorcycleServiceMock.Setup(m => m.CreateMotorcycleAsync(motorcycle, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync(motorcycle);
            _mapperMock.Setup(m => m.Map<MotorcycleDto>(motorcycle)).Returns(motorcycleDto);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(motorcycleDto, result.Motorcycle);
            Assert.Equal("Motorcycle registered successfully.", result.Message);
        }

        [Fact]
        public async Task Handle_UnsuccessfulCreation_ThrowsException()
        {
            // Arrange
            var command = new CreateMotoCommand
            {
                LicensePlate = "ABC123",
                Model = "Test Model",
                Year = 2022
            };

            var motorcycle = new Motorcycle
            {
                LicensePlate = command.LicensePlate,
                Model = command.Model,
                Year = command.Year
            };

            _mapperMock.Setup(m => m.Map<Motorcycle>(command)).Returns(motorcycle);
            _motorcycleServiceMock.Setup(m => m.CreateMotorcycleAsync(motorcycle, It.IsAny<CancellationToken>()))
                                  .ReturnsAsync((Motorcycle)null);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}
