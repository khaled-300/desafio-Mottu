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
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IDeliveryUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IDeliveryUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetUserByIdQueryHandler(_mapperMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_GetUserById_ReturnsDeliveryUserDto()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 1 };
            var user = new DeliveryUser
            {
                Id = 1,
                Name = "John Doe",
                CNPJ = "12345678901234",
                DateOfBirth = new DateTime(1990, 1, 1),
                LicenseNumber = "ABC123",
                LicenseType = LicenseType.A,
                LicenseImageURL = "http://example.com/license.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
            var userDto = new DeliveryUserDto
            {
                Id = 1,
                Name = "John Doe",
                CNPJ = "12345678901234",
                DateOfBirth = new DateTime(1990, 1, 1),
                LicenseNumber = "ABC123",
                LicenseType = LicenseType.A,
                LicenseImageURL = "http://example.com/license.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map<DeliveryUserDto>(user))
                       .Returns(userDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto, result);
        }

        [Fact]
        public async Task Handle_GetUserById_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 1 };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DeliveryUser)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
