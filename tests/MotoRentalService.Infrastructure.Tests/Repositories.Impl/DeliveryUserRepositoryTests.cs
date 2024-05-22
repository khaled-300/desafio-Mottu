using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Infrastructure.Repositories.Impl;


namespace MotoRentalService.Infrastructure.Tests.Repositories.Impl
{
    public class DeliveryUserRepositoryTests : IAsyncLifetime
    {
        private readonly DeliveryUserRepository _repository;
        private readonly AppDbContext _context;

        public DeliveryUserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseDeliveryUser")
                .Options;

            _context = new AppDbContext(options);
            _repository = new DeliveryUserRepository(_context);
        }

        public async Task InitializeAsync()
        {
            _context.Database.EnsureCreated();
            Task.WaitAll(SeedData());
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
            var users = new List<DeliveryUser>
            {
                new DeliveryUser
                {
                    Id = 1,
                    CNPJ = "12345678901234",
                    LicenseNumber = "LN123",
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    DateOfBirth = DateTime.Today.AddDays(-30).ToUniversalTime(),
                    LicenseImageFullName = "test.png",
                    LicenseImageURL = "path/to/test.png",
                    LicenseType = Domain.ValueObjects.LicenseType.A,
                    Name = "Test Name",
                    Status = Domain.ValueObjects.UserStatus.Approved,
                    UpdatedAt = DateTime.UtcNow,
                },
                new DeliveryUser
                {
                    Id = 2,
                    CNPJ = "23456789012345",
                    LicenseNumber = "LN124",
                    UserId = 2,
                    CreatedAt = DateTime.UtcNow,
                    DateOfBirth = DateTime.Today.AddDays(-30).ToUniversalTime(),
                    LicenseImageFullName = "test.png",
                    LicenseImageURL = "path/to/test.png",
                    LicenseType = Domain.ValueObjects.LicenseType.A,
                    Name = "Test Name",
                    Status = Domain.ValueObjects.UserStatus.Approved,
                    UpdatedAt = null,
                }
            };

            _context.DeliveryUser.AddRange(users);
            await _context.SaveChangesAsync(CancellationToken.None);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdAsync(99, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedUsers()
        {
            // Act
            var result = await _repository.GetAllAsync(1, 1, CancellationToken.None);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            // Arrange
            var newUser = new DeliveryUser
            {
                Id = 3,
                CNPJ = "34567890123456",
                LicenseNumber = "LN125",
                UserId = 3,
                CreatedAt = DateTime.UtcNow,
                DateOfBirth = DateTime.Today.AddDays(-30).ToUniversalTime(),
                LicenseImageFullName = "test.png",
                LicenseImageURL = "path/to/test.png",
                LicenseType = Domain.ValueObjects.LicenseType.A,
                Name = "Test Name",
                Status = Domain.ValueObjects.UserStatus.Approved,
                UpdatedAt = null,
            };

            // Act
            var result = await _repository.AddAsync(newUser, CancellationToken.None);

            // Assert
            Assert.Equal(newUser.Id, result.Id);
            var userInDb = await _context.DeliveryUser.FindAsync(3);
            Assert.NotNull(userInDb);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var user = await _context.DeliveryUser.FindAsync(1);
            user.CNPJ = "UpdatedCNPJ";

            // Act
            await _repository.UpdateAsync(user, CancellationToken.None);

            // Assert
            var updatedUser = await _context.DeliveryUser.FindAsync(1);
            Assert.Equal("UpdatedCNPJ", updatedUser.CNPJ);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldRemoveUser_WhenUserExists()
        {
            // Act
            await _repository.DeleteByIdAsync(1, CancellationToken.None);

            // Assert
            var userInDb = await _context.DeliveryUser.FindAsync(1);
            Assert.Null(userInDb);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _repository.DeleteByIdAsync(99, CancellationToken.None));
        }

        [Fact]
        public async Task ExistsByCNPJAsync_ShouldReturnTrue_WhenCNPJExists()
        {
            // Act
            var result = await _repository.ExistsByCNPJAsync("12345678901234", CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByCNPJAsync_ShouldReturnFalse_WhenCNPJDoesNotExist()
        {
            // Act
            var result = await _repository.ExistsByCNPJAsync("NonExistingCNPJ", CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsByLicenseNumberAsync_ShouldReturnTrue_WhenLicenseNumberExists()
        {
            // Act
            var result = await _repository.ExistsByLicenseNumberAsync("LN123", CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsByLicenseNumberAsync_ShouldReturnFalse_WhenLicenseNumberDoesNotExist()
        {
            // Act
            var result = await _repository.ExistsByLicenseNumberAsync("NonExistingLN", CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUser_WhenUserIdExists()
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
