using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Infrastructure.Repositories.Impl;

namespace MotoRentalService.Infrastructure.Tests.Repositories.Impl
{
    public class PlansRepositoryTests : IAsyncLifetime
    {

        private readonly PlansRepository _repository;
        private readonly AppDbContext _context;

        public PlansRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabasePlans")
                .Options;

            _context = new AppDbContext(options);
            _repository = new PlansRepository(_context);
        }

        public async Task InitializeAsync()
        {
            _context.Database.EnsureCreated();
            await SeedData();
        }

        public Task DisposeAsync()
        {
            // Clean up if needed
            _context.Database.EnsureDeleted();
            _context.Dispose();
            return Task.CompletedTask;
        }

        private async Task SeedData()
        {
            var plans = new List<RentalPlans>
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
                new RentalPlans {
                    Id = 3,
                    Name = "Plan B",
                    DurationInDays = 20,
                    DailyRate = 30,
                    IsActive = true 
                },
                new RentalPlans {
                    Id = 4,
                    Name = "Plan B",
                    DurationInDays = 10,
                    DailyRate = 10,
                    IsActive= true,
                }
            };

            _context.RentalPlans.AddRange(plans);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task AddPlanAsync_ShouldAddPlan()
        {
            // Arrange
            var newPlan = new RentalPlans { Id = 5, Name = "Plan C", DailyRate = 30, DurationInDays = 1, IsActive = true };

            // Act
            var result = await _repository.AddPlanAsync(newPlan, CancellationToken.None);

            // Assert
            Assert.Equal(newPlan.Id, result.Id);
            var planInDb = await _context.RentalPlans.FindAsync(3);
            Assert.NotNull(planInDb);
        }

        [Fact]
        public async Task GetPlanByIdAsync_ShouldReturnPlan_WhenPlanExists()
        {
            // Act
            var result = await _repository.GetPlanByIdAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetPlanByIdAsync_ShouldReturnNull_WhenPlanDoesNotExist()
        {
            // Act
            var result = await _repository.GetPlanByIdAsync(99, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPlans()
        {
            // Act
            var result = await _repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public async Task UpdatePlanAsync_ShouldUpdatePlan()
        {
            // Arrange
            var plan = await _context.RentalPlans.FindAsync(1);
            plan.Name = "Updated Plan A";

            // Act
            var result = await _repository.UpdatePlanAsync(plan, CancellationToken.None);

            // Assert
            Assert.Equal("Updated Plan A", result.Name);
            var updatedPlan = await _context.RentalPlans.FindAsync(1);
            Assert.Equal("Updated Plan A", updatedPlan.Name);
        }

        [Fact]
        public async Task DeletePlanAsync_ShouldRemovePlan_WhenPlanExists()
        {
            // Act
            await _repository.DeletePlanAsync(1, CancellationToken.None);

            // Assert
            var planInDb = await _context.RentalPlans.FindAsync(1);
            Assert.Null(planInDb);
        }

        [Fact]
        public async Task DeletePlanAsync_ShouldThrowException_WhenPlanDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _repository.DeletePlanAsync(99, CancellationToken.None));
        }
    }
}
