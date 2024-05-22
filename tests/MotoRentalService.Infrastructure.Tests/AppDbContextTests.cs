using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Infrastructure.Tests
{
    public class AppDbContextTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public AppDbContextTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldSetCreatedAt_ForNewEntities()
        {
            // Arrange
            using var context = new AppDbContext(_dbContextOptions);
            var user = new Users { Email = "newuser@example.com", Password = "password123", Role = UserRole.User };

            // Act
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(default, user.CreatedAt);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldSetUpdatedAt_ForModifiedEntities()
        {
            // Arrange
            using var context = new AppDbContext(_dbContextOptions);
            var user = new Users { Email = "existinguser@example.com", Password = "password123", Role = UserRole.User, CreatedAt = DateTime.UtcNow };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            user.Email = "updateduser@example.com";
            await context.SaveChangesAsync();

            // Assert
            Assert.NotEqual(default, user.UpdatedAt);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldUpdateDeliveryUserStatus_WhenLicenseImageURLChanges()
        {
            // Arrange
            using var context = new AppDbContext(_dbContextOptions);
            var user = new Users { Email = "deliveryuser@example.com", Password = "password123", Role = UserRole.User, CreatedAt = DateTime.UtcNow };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var deliveryUser = new DeliveryUser
            {
                UserId = user.Id,
                Name = "Delivery User",
                CNPJ = "12345678901234",
                DateOfBirth = DateTime.Parse("1990-01-01"),
                LicenseNumber = "LIC12345",
                LicenseType = LicenseType.A,
                CreatedAt = DateTime.UtcNow
            };

            context.DeliveryUser.Add(deliveryUser);
            await context.SaveChangesAsync();

            // Act
            deliveryUser.LicenseImageURL = "newurl";
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(UserStatus.Approved, deliveryUser.Status);
        }
    }
}
