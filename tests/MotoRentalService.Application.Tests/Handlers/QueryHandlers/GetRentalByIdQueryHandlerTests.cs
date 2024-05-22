using AutoMapper;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.QueryHandlers;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Application.Tests.Handlers.QueryHandlers
{
    public class GetRentalByIdQueryHandlerTests
    {

        private readonly Mock<IRentalRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetRentalByIdQueryHandler _handler;

        public GetRentalByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IRentalRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetRentalByIdQueryHandler(_mapperMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_GetRentalById_ReturnsRentalDto()
        {
            // Arrange
            var query = new GetRentalByIdQuery { RentalId = 1 };
            var rental = new Rental
            {
                Id = 1,
                MotorcycleId = 1,
                UserId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                ExpectedEndDate = DateTime.UtcNow.AddDays(1),
                DailyRate = 10.00m,
                TotalPrice = 20.00m,
                Status = RentalStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            var rentalDto = new RentalDto
            {
                Id = 1,
                MotorcycleId = 1,
                UserId = 1,
                RentalPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                ExpectedEndDate = DateTime.UtcNow.AddDays(1),
                DailyRate = 10.00m,
                TotalPrice = 20.00m,
                Status = RentalStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(query.RentalId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(rental);
            _mapperMock.Setup(mapper => mapper.Map<RentalDto>(rental))
                       .Returns(rentalDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(rentalDto, result);
        }

        [Fact]
        public async Task Handle_GetRentalById_ReturnsNull_WhenRentalNotFound()
        {
            // Arrange
            var query = new GetRentalByIdQuery { RentalId = 1 };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(query.RentalId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Rental)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ThrowsArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            GetRentalByIdQuery? query = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(query!, CancellationToken.None));
        }
    }
}
