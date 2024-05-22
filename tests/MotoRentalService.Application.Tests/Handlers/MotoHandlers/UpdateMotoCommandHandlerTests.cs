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
    public class UpdateMotoCommandHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMotorcycleService> _motorcycleServiceMock;
        private readonly UpdateMotoCommandHandler _handler;
        private readonly Mock<IValidator<UpdateMotoCommand>> _validator;

        public UpdateMotoCommandHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _motorcycleServiceMock = new Mock<IMotorcycleService>();
            _validator = new Mock<IValidator<UpdateMotoCommand>>();
            _handler = new UpdateMotoCommandHandler(_mapperMock.Object, _motorcycleServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_MotorcycleFoundAndUpdated_ReturnsSuccessResult()
        {
            // Arrange
            var command = new UpdateMotoCommand { Id = 1, LicensePlate = "XYZ789" };
            var motorcycleDto = new MotorcycleDto { Id = 1, LicensePlate = "XYZ789" };
            var motorcycle = new Motorcycle { Id = 1, LicensePlate = "ABC123", Model = "Test Model", Year = 2022 };

            _mapperMock.Setup(m => m.Map<MotorcycleDto>(command)).Returns(motorcycleDto);
            _motorcycleServiceMock.Setup(s => s.GetMotorcycleById(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync(motorcycle);
            _motorcycleServiceMock.Setup(s => s.UpdateMotorcycleAsync(motorcycle, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<MotorcycleDto>(motorcycle)).Returns(motorcycleDto);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Motorcycle updated successfully.", result.Message);
            Assert.Equal(motorcycleDto, result.Motorcycle);
        }

        [Fact]
        public async Task Handle_MotorcycleNotFound_ReturnsFailureResult()
        {
            // Arrange
            var command = new UpdateMotoCommand { Id = 1, LicensePlate = "XYZ789" };
            var motorcycleDto = new MotorcycleDto { Id = 1, LicensePlate = "XYZ789" };

            _mapperMock.Setup(m => m.Map<MotorcycleDto>(command)).Returns(motorcycleDto);
            _motorcycleServiceMock.Setup(s => s.GetMotorcycleById(command.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Motorcycle)null);
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
