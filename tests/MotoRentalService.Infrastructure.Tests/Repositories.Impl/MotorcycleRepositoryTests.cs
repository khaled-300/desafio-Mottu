using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Infrastructure.Repositories.Impl;

namespace MotoRentalService.Infrastructure.Tests.Repositories.Impl
{
    public class MotorcycleRepositoryTests : IAsyncLifetime
    {
        private readonly MotorcycleRepository _repository;
        private readonly AppDbContext _context;

        public MotorcycleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);
            _repository = new MotorcycleRepository(_context);
        }

        public async Task InitializeAsync()
        {
            _context.Database.EnsureCreated();
            await SeedData();
        }

        public Task DisposeAsync()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            return Task.CompletedTask;
        }

        private async Task SeedData()
        {
            var motorcycles = new List<Motorcycle>
            {
                new Motorcycle
                {
                    Id = 1,
                    Model = "Model1",
                    Year = 2020,
                    LicensePlate = "ABC123",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsRented = false
                },
                new Motorcycle
                {
                    Id = 2,
                    Model = "Model2",
                    Year = 2021,
                    LicensePlate = "XYZ789",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsRented = true
                },
                new Motorcycle
                {
                    Id = 3,
                    Model = "Model3",
                    Year = 2021,
                    LicensePlate = "XYZ710",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsRented = false
                }
            };

            _context.Motorcycles.AddRange(motorcycles);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddMotorcycle()
        {
            // Arrange
            var newMotorcycle = new Motorcycle
            {
                Id = 4,
                Model = "Model3",
                Year = 2022,
                LicensePlate = "LMN456",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _repository.CreateAsync(newMotorcycle, CancellationToken.None);

            // Assert
            Assert.Equal(newMotorcycle.Id, result);
            var motoInDb = await _context.Motorcycles.FindAsync(3);
            Assert.NotNull(motoInDb);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedMotorcycles()
        {
            // Act
            var result = await _repository.GetAllAsync(1, 1, CancellationToken.None);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMotorcycle_WhenMotorcycleExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenMotorcycleDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdAsync(99, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByLicensePlateAsync_ShouldReturnMotorcycle_WhenLicensePlateExists()
        {
            // Act
            var result = await _repository.GetByLicensePlateAsync("ABC123", CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ABC123", result.LicensePlate);
        }

        [Fact]
        public async Task GetByLicensePlateAsync_ShouldReturnNull_WhenLicensePlateDoesNotExist()
        {
            // Act
            var result = await _repository.GetByLicensePlateAsync("NONEXISTENT", CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateMotorcycle()
        {
            // Arrange
            var moto = await _context.Motorcycles.FindAsync(1);
            moto.Model = "UpdatedModel";

            // Act
            await _repository.UpdateAsync(moto, CancellationToken.None);

            // Assert
            var updatedMoto = await _context.Motorcycles.FindAsync(1);
            Assert.Equal("UpdatedModel", updatedMoto.Model);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveMotorcycle_WhenMotorcycleExists()
        {
            // Arrange
            var moto = await _context.Motorcycles.FindAsync(1);

            // Act
            await _repository.DeleteAsync(moto, CancellationToken.None);

            // Assert
            var motoInDb = await _context.Motorcycles.FindAsync(1);
            Assert.Null(motoInDb);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenMotorcycleDoesNotExist()
        {
            // Arrange
            var moto = new Motorcycle { Id = 99 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _repository.DeleteAsync(moto, CancellationToken.None));
            Assert.Equal("Attempted to delete an entity that does not exist in the store.", exception.Message);
        }
    }
}
