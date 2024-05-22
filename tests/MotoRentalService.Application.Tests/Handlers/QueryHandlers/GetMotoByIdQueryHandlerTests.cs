using AutoMapper;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.QueryHandlers;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.Tests.Handlers.QueryHandlers
{
    public class GetMotoByIdQueryHandlerTests
    {
        private readonly Mock<IMotorcycleRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetMotoPageQueryHandler _handler;

        public GetMotoByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IMotorcycleRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetMotoPageQueryHandler(_mapperMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_GetMotoById_ReturnsMotorcycleDto()
        {
            // Arrange
            var query = new GetMotoByIdQuery(1);
            var motorcycle = new Motorcycle { Id = 1, LicensePlate = "ABC123", Model = "Model1", Year = 2002, IsRented = true };
            var motorcycleDto = new MotorcycleDto { Id = 1, LicensePlate = "ABC123", Model = "Model1", Year = 2002, IsRented = true };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(query.MotorcycleId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(motorcycle);
            _mapperMock.Setup(mapper => mapper.Map<MotorcycleDto?>(motorcycle))
                       .Returns(motorcycleDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(motorcycleDto, result);
        }

        [Fact]
        public async Task Handle_GetMotoById_ReturnsNull_WhenMotorcycleNotFound()
        {
            // Arrange
            var query = new GetMotoByIdQuery(1);

            _repositoryMock.Setup(repo => repo.GetByIdAsync(query.MotorcycleId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Motorcycle)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
