using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.ValueObjects;
using MotoRentalService.Infrastructure.Repositories.Impl;

namespace MotoRentalService.Infrastructure.Tests.Repositories.Impl
{
    public class RentalRepositoryTests : IAsyncLifetime
    {

        private readonly RentalRepository _repository;
        private readonly AppDbContext _context;

        public RentalRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseRental")
                .Options;

            _context = new AppDbContext(options);
            _repository = new RentalRepository(_context);
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
            var rentalPlans = new List<RentalPlans>
            {
                new RentalPlans {
                    Id = 1,
                    Name = "Plan A",
                    DurationInDays = 7,
                    DailyRate = 50,
                    IsActive = true,
                },
                new RentalPlans {
                    Id = 2,
                    Name = "Plan B",
                    DurationInDays = 15,
                    DailyRate = 40,
                    IsActive= false,
                },
            };

            _context.RentalPlans.AddRange(rentalPlans);
            await _context.SaveChangesAsync(CancellationToken.None);

            var rentals = new List<Rental>
            {
                new Rental { 
                    Id = 1, 
                    UserId = 1, MotorcycleId = 1, 
                    RentalPlanId = 1, 
                    StartDate = DateTime.UtcNow, 
                    EndDate = DateTime.UtcNow.AddDays(1), 
                    ExpectedEndDate = DateTime.UtcNow.AddDays(1), 
                    DailyRate = 10.0m, 
                    TotalPrice = 10.0m, 
                    Status = RentalStatus.Active 
                },
                new Rental { 
                    Id = 2, 
                    UserId = 2, 
                    MotorcycleId = 2, 
                    RentalPlanId = 2, 
                    StartDate = DateTime.UtcNow, 
                    EndDate = DateTime.UtcNow.AddDays(2), 
                    ExpectedEndDate = DateTime.UtcNow.AddDays(2), 
                    DailyRate = 20.0m, 
                    TotalPrice = 40.0m, 
                    Status = RentalStatus.Active 
                }
            };

            _context.Rentals.AddRange(rentals);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task AddRentAsync_ShouldAddRent()
        {
            // Arrange
            var newRental = new Rental { Id = 3, UserId = 3, MotorcycleId = 3, RentalPlanId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), ExpectedEndDate = DateTime.UtcNow.AddDays(1), DailyRate = 10.0m, TotalPrice = 10.0m, Status = RentalStatus.Active };

            // Act
            var result = await _repository.AddRentAsync(newRental, CancellationToken.None);

            // Assert
            Assert.Equal(newRental.Id, result.Id);
            var rentalInDb = await _context.Rentals.FindAsync(3);
            Assert.NotNull(rentalInDb);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRent_WhenRentExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenRentDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdAsync(99, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedRents()
        {
            // Act
            var result = await _repository.GetAllAsync(1, 1, CancellationToken.None);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateRent()
        {
            // Arrange
            var rent = await _context.Rentals.FindAsync(1);
            rent.TotalPrice = 15.0m;

            // Act
            await _repository.UpdateAsync(rent, CancellationToken.None);

            // Assert
            var updatedRent = await _context.Rentals.FindAsync(1);
            Assert.Equal(15.0m, updatedRent.TotalPrice);
        }

        [Fact]
        public async Task DeleteRentAsync_ShouldRemoveRent_WhenRentExists()
        {
            // Act
            var rent = await _context.Rentals.FindAsync(1);
            await _repository.DeleteRentAsync(rent, CancellationToken.None);

            // Assert
            var rentInDb = await _context.Rentals.FindAsync(1);
            Assert.Null(rentInDb);
        }

        [Fact]
        public async Task DeleteRentAsync_ShouldThrowException_WhenRentDoesNotExist()
        {
            // Act & Assert
            var rent = new Rental { Id = 99 };
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _repository.DeleteRentAsync(rent, CancellationToken.None));
        }

        [Fact]
        public async Task DeleteRentByUserIdAsync_ShouldRemoveRent_WhenRentExists()
        {
            // Act
            await _repository.DeleteRentByUserIdAsync(1, CancellationToken.None);

            // Assert
            var rentInDb = await _context.Rentals.FindAsync(1);
            Assert.Null(rentInDb);
        }

        [Fact]
        public async Task DeleteRentByUserIdAsync_ShouldThrowException_WhenRentDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _repository.DeleteRentByUserIdAsync(99, CancellationToken.None));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnRent_WhenUserIdExists()
        {
            // Act
            var result = await _repository.GetByUserIdAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnNull_WhenUserIdDoesNotExist()
        {
            // Act
            var result = await _repository.GetByUserIdAsync(99, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
