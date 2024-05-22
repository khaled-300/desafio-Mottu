using Microsoft.EntityFrameworkCore;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Infrastructure.Repositories.Impl;

namespace MotoRentalService.Infrastructure.Tests.Repositories.Impl
{
    public class UserAuthenticationRepositoryTests : IAsyncLifetime
    {
        private readonly UserAuthenticationRepository _repository;
        private readonly AppDbContext _context;

        public UserAuthenticationRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseUserAuthentication")
                .Options;

            _context = new AppDbContext(options);
            _repository = new UserAuthenticationRepository(_context);
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
            var users = new List<Users>
            {
                new Users { 
                    Id = 1, 
                    Email = "user1@example.com", 
                    Password = "hash1", 
                    CreatedAt = DateTime.UtcNow, 
                    Role = Domain.ValueObjects.UserRole.Admin, 
                    UpdatedAt = null 
                },
                new Users { 
                    Id = 2, 
                    Email = "user2@example.com", 
                    Password = "hash2", 
                    CreatedAt = DateTime.UtcNow, 
                    Role = Domain.ValueObjects.UserRole.User, 
                    UpdatedAt = null 
                }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUser()
        {
            // Arrange
            var newUser = new Users { Id = 3, Email = "user3@example.com", Password = "hash3", CreatedAt = DateTime.UtcNow };

            // Act
            await _repository.AddUserAsync(newUser, CancellationToken.None);

            // Assert
            var userInDb = await _context.Users.FindAsync(new object[] { newUser.Id });
            Assert.NotNull(userInDb);
            Assert.Equal(newUser.Email, userInDb.Email);
        }

        [Fact]
        public async Task DeleteUserByIdAsync_ShouldDeleteUser_WhenUserExists()
        {
            // Act
            await _repository.DeleteUserByIdAsync(1, CancellationToken.None);

            // Assert
            var userInDb = await _context.Users.FindAsync(new object[] { 1 });
            Assert.Null(userInDb);
        }

        [Fact]
        public async Task DeleteUserByIdAsync_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _repository.DeleteUserByIdAsync(99, CancellationToken.None));
            Assert.Equal("User was not found!", exception.Message);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Act
            var user = await _repository.GetUserByEmailAsync("user1@example.com", CancellationToken.None);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("user1@example.com", user.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var user = await _repository.GetUserByEmailAsync("nonexistent@example.com", CancellationToken.None);

            // Assert
            Assert.Null(user);
        }
    }
}
