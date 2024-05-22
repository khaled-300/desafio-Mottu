using AutoMapper;
using Moq;
using MotoRentalService.Application.Dtos;
using MotoRentalService.Application.MediatR.Queries;
using MotoRentalService.Application.MediatR.QueryHandlers;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Application.Tests.Handlers.QueryHandlers
{
    public class GetMotoPageQueryHandlerTests
    {

        private readonly Mock<IMotorcycleRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetMotoPageQueryHandler _handler;

        public GetMotoPageQueryHandlerTests()
        {
            _repositoryMock = new Mock<IMotorcycleRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetMotoPageQueryHandler(_mapperMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_GetMotoPage_ReturnsMotorcycleDtos()
        {
            // Arrange
            var query = new GetMotoPageQuery(1, 10);
            var motorcycles = new List<Motorcycle>
            {
                new Motorcycle { Id = 1, LicensePlate = "ABC123", Model = "Model1", Year = 2002 , IsRented = true },
                new Motorcycle { Id = 2, LicensePlate = "DEF456", Model = "Model2", Year = 2002 , IsRented = false}
            };
            var motorcycleDtos = new List<MotorcycleDto>
            {
                new MotorcycleDto { Id = 1, LicensePlate = "ABC123", Model = "Model1", Year = 2002 ,IsRented = true },
                new MotorcycleDto { Id = 2, LicensePlate = "DEF456", Model = "Model2", Year = 2002 ,IsRented = false }
            };

            _repositoryMock.Setup(repo => repo.GetAllAsync(query.PageNumber, query.PageSize, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(motorcycles);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<MotorcycleDto>>(motorcycles))
                       .Returns(motorcycleDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(motorcycleDtos, result);
        }
    }
}
