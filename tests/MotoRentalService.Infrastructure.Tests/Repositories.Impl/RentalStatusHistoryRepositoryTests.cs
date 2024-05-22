using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.ValueObjects;
using MotoRentalService.Infrastructure.Repositories.Impl;

namespace MotoRentalService.Infrastructure.Tests.Repositories.Impl
{
    public class RentalStatusHistoryRepositoryTests : IAsyncLifetime
    {
        private readonly RentalStatusHistoryRepository _repository;
        private readonly AppDbContext _context;

        public RentalStatusHistoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseRentalStatusHistory")
                .Options;

            _context = new AppDbContext(options);
            _repository = new RentalStatusHistoryRepository(_context);
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
            var rentals = new List<Rental>
            {
                new Rental { 
                    Id = 1, 
                    UserId = 1, 
                    MotorcycleId = 1, 
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
        public async Task AddRentalStatusHistoryAsync_ShouldAddRentalStatusHistory()
        {
            // Arrange
            var newRentalStatusHistory = new Domain.Entities.RentalStatusHistory
            {
                Id = 1,
                RentalId = 1,
                Status = RentalStatus.Pending,
                StatusChangedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _repository.AddRentalStatusHistoryAsync(newRentalStatusHistory, CancellationToken.None);

            // Assert
            Assert.Equal(newRentalStatusHistory.Id, result.Id);
            var rentalStatusInDb = await _context.RentalStatusHistories.FindAsync(1);
            Assert.NotNull(rentalStatusInDb);
        }

    }
}
