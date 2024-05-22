using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.CommandHandlers.RentalHandlers;
using MotoRentalService.Application.MediatR.Commands.Rental;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;

namespace MotoRentalService.Application.Tests.Handlers.RentalHandlers
{
    public class CreateRentalCommandHandlerTests
    {
        private readonly Mock<IRentalService> _rentalServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateRentalCommandHandler _handler;
        private readonly Mock<IValidator<CreateRentalCommand>> _validator;

        public CreateRentalCommandHandlerTests()
        {
            _rentalServiceMock = new Mock<IRentalService>();
            _mapperMock = new Mock<IMapper>();
            _validator = new Mock<IValidator<CreateRentalCommand>>();
            _handler = new CreateRentalCommandHandler(_mapperMock.Object, _rentalServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_RentalCreatedSuccessfully_ReturnsSuccessResult()
        {
            // Arrange
            var command = new CreateRentalCommand
            {
                EndDate = DateTime.UtcNow,
                MotorcycleId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                UserId = 1
            };

            var rental = new Rental()
            {
                UserId = command.UserId,
                MotorcycleId = command.MotorcycleId,
                StartDate = command.StartDate,
                RentalPlanId = command.RentalPlanId,
                EndDate = command.EndDate,
                CreatedAt = DateTime.UtcNow,
                Id = 1,
                Status = Domain.ValueObjects.RentalStatus.Pending,
                UpdatedAt = null,
                TotalPrice = 230,
                DailyRate = 30,
                ExpectedEndDate = DateTime.Today.AddDays(7),
            };
            var newRental = new Rental();
            var rentalDto = new RentalDto()
            {
                Id = rental.Id,
                UpdatedAt = rental.UpdatedAt,
                Status = rental.Status,
                CreatedAt = rental.CreatedAt,
                DailyRate = rental.DailyRate,
                EndDate = rental.EndDate,
                ExpectedEndDate = DateTime.UtcNow,
                MotorcycleId = command.MotorcycleId,
                StartDate = command.StartDate,
                RentalPlanId = command.RentalPlanId,
                TotalPrice = 230,
                UserId = command.UserId
            };

            _mapperMock.Setup(m => m.Map<Rental>(command)).Returns(rental);
            _rentalServiceMock.Setup(service => service.RentMotorcycleAsync(rental, It.IsAny<CancellationToken>())).ReturnsAsync(newRental);
            _mapperMock.Setup(m => m.Map<RentalDto>(newRental)).Returns(rentalDto);
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Rent was created successfully", result.Message);
            Assert.Equal(rentalDto, result.Rental);
        }

        [Fact]
        public async Task Handle_RentalCreationFailed_ThrowsException()
        {
            // Arrange
            var command = new CreateRentalCommand();
            var rental = new Rental();

            _mapperMock.Setup(m => m.Map<Rental>(command)).Returns(rental);
            _rentalServiceMock.Setup(service => service.RentMotorcycleAsync(rental, It.IsAny<CancellationToken>()))
                              .ThrowsAsync(new Exception("Rental creation failed"));
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}
