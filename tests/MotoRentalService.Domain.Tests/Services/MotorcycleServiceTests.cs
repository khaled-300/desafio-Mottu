using Moq;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.Services;

namespace MotoRentalService.Domain.Tests.Services
{
    public class MotorcycleServiceTests
    {
        private readonly Mock<IMotorcycleRepository> _repositoryMock;
        private readonly MotorcycleService _motorcycleService;

        public MotorcycleServiceTests()
        {
            _repositoryMock = new Mock<IMotorcycleRepository>();
            _motorcycleService = new MotorcycleService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateMotorcycleAsync_ShouldCreateMotorcycle_WhenValidMotorcycle()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Year = 2022,
                Model = "Test Model",
                LicensePlate = "ABC123",
                IsRented = false
            };
            _repositoryMock.Setup(r => r.GetByLicensePlateAsync(motorcycle.LicensePlate, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Motorcycle?)null);
            _repositoryMock.Setup(r => r.CreateAsync(motorcycle, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _motorcycleService.CreateMotorcycleAsync(motorcycle, CancellationToken.None);

            // Assert
            Assert.Equal(1, result.Id);
            _repositoryMock.Verify(r => r.CreateAsync(motorcycle, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateMotorcycleAsync_ShouldThrowInvalidOperationException_WhenLicensePlateExists()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Year = 2022,
                Model = "Test Model",
                LicensePlate = "ABC123",
                IsRented = false
            };
            _repositoryMock.Setup(r => r.GetByLicensePlateAsync(motorcycle.LicensePlate, It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _motorcycleService.CreateMotorcycleAsync(motorcycle, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteMotorcycleAsync_ShouldDeleteMotorcycle_WhenMotorcycleExists()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = 1,
                Year = 2022,
                Model = "Test Model",
                LicensePlate = "ABC123",
                IsRented = false
            };
            _repositoryMock.Setup(r => r.GetByIdAsync(motorcycle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            // Act
            await _motorcycleService.DeleteMotorcycleAsync(motorcycle.Id, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(motorcycle, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteMotorcycleAsync_ShouldThrowKeyNotFoundException_WhenMotorcycleDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Motorcycle?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _motorcycleService.DeleteMotorcycleAsync(1, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateMotorcycleAsync_ShouldUpdateMotorcycle()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = 1,
                Year = 2022,
                Model = "Updated Model",
                LicensePlate = "ABC123",
                IsRented = false
            };

            // Act
            await _motorcycleService.UpdateMotorcycleAsync(motorcycle, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(motorcycle, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetMotorcycleById_ShouldReturnMotorcycle_WhenMotorcycleExists()
        {
            // Arrange
            var motorcycle = new Motorcycle
            {
                Id = 1,
                Year = 2022,
                Model = "Test Model",
                LicensePlate = "ABC123",
                IsRented = false
            };
            _repositoryMock.Setup(r => r.GetByIdAsync(motorcycle.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(motorcycle);

            // Act
            var result = await _motorcycleService.GetMotorcycleById(motorcycle.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(motorcycle.Id, result.Id);
        }

        [Fact]
        public async Task GetMotorcycleById_ShouldReturnNull_WhenMotorcycleDoesNotExist()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Motorcycle?)null);

            // Act
            var result = await _motorcycleService.GetMotorcycleById(1, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
