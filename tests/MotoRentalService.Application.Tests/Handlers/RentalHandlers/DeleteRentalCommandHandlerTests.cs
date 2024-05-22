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
    public class DeleteRentalCommandHandlerTests
    {
        private readonly Mock<IRentalService> _rentalServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly DeleteRentalCommandHandler _handler;
        private readonly Mock<IValidator<DeleteRentalCommand>> _validator;

        public DeleteRentalCommandHandlerTests()
        {
            _rentalServiceMock = new Mock<IRentalService>();
            _mapperMock = new Mock<IMapper>();
            _validator = new Mock<IValidator<DeleteRentalCommand>>();
            _handler = new DeleteRentalCommandHandler(_mapperMock.Object, _rentalServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task Handle_RentalDeletionSuccessful_ReturnsSuccessResult()
        {
            // Arrange
            var command = new DeleteRentalCommand
            {
                Id = 1
            };

            var rental = new Rental();

            _mapperMock.Setup(m => m.Map<Rental>(command)).Returns(rental);
            _rentalServiceMock.Setup(service => service.MarkRentalAsCompletedAsync(rental.Id, It.IsAny<CancellationToken>()));
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Plan was marked as completed successfully", result.Message);
        }

        [Fact]
        public async Task Handle_RentalDeletionFailed_ThrowsException()
        {
            // Arrange
            var command = new DeleteRentalCommand
            {
                Id = 0
            };

            var rental = new Rental();

            _mapperMock.Setup(m => m.Map<Rental>(command)).Returns(rental);
            _rentalServiceMock.Setup(service => service.MarkRentalAsCompletedAsync(rental.Id, It.IsAny<CancellationToken>()))
                              .ThrowsAsync(new Exception("Rental deletion failed"));
            _validator.Setup(validator => validator.ValidateAsync(command, CancellationToken.None)).ReturnsAsync(new ValidationResult());
            // Act & Assert
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(1, result.Errors.Count());
        }
    }
}
