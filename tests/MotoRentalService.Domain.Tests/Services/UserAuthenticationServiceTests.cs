using Moq;
using MotoRentalService.Domain.Interfaces.repository;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Services;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.ValueObjects;

namespace MotoRentalService.Domain.Tests.Services
{
    public class UserAuthenticationServiceTests
    {
        private readonly Mock<IUserAuthenticationRepository> _mockUserAuthRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPasswordHasherService> _mockPasswordHasherService;
        private readonly UserAuthenticationService _userAuthService;

        public UserAuthenticationServiceTests()
        {
            _mockUserAuthRepository = new Mock<IUserAuthenticationRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _mockPasswordHasherService = new Mock<IPasswordHasherService>();
            _userAuthService = new UserAuthenticationService(_mockUserAuthRepository.Object, _mockTokenService.Object, _mockPasswordHasherService.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser()
        {
            // Arrange
            var user = new Users { Id = 1, Email = "test@example.com", Password = "123123", Role = UserRole.User };
            _mockUserAuthRepository.Setup(repo => repo.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync((Users)null);
            _mockPasswordHasherService.Setup(service => service.HashPassword(It.IsAny<string>())).Returns("hashed_password");
            _mockUserAuthRepository.Setup(repo => repo.AddUserAsync(user, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockTokenService.Setup(service => service.GenerateTokenAsync(user.Id, user.Email, user.Role, It.IsAny<CancellationToken>())).ReturnsAsync("token");

            // Act
            var result = await _userAuthService.RegisterUserAsync(user, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("token", result);
            _mockUserAuthRepository.Verify(repo => repo.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()), Times.Once);
            _mockUserAuthRepository.Verify(repo => repo.AddUserAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _mockTokenService.Verify(service => service.GenerateTokenAsync(user.Id, user.Email, user.Role, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            var user = new Users { Id = 1, Email = "test@example.com", Password = "password", Role = UserRole.User };
            _mockUserAuthRepository.Setup(repo => repo.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _userAuthService.RegisterUserAsync(user, CancellationToken.None));
            Assert.Equal("Users is already registerd", ex.Message);
            _mockUserAuthRepository.Verify(repo => repo.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()), Times.Once);
            _mockPasswordHasherService.Verify(service => service.HashPassword(It.IsAny<string>()), Times.Never);
            _mockUserAuthRepository.Verify(repo => repo.AddUserAsync(It.IsAny<Users>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockTokenService.Verify(service => service.GenerateTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new Users { Id = 1, Email = "test@example.com", Password = "hashed_password", Role = UserRole.User };
            _mockUserAuthRepository.Setup(repo => repo.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockPasswordHasherService.Setup(service => service.VerifyPassword(user.Password, "password")).Returns(true);
            _mockTokenService.Setup(service => service.GenerateTokenAsync(user.Id, user.Email, user.Role, It.IsAny<CancellationToken>())).ReturnsAsync("token");

            // Act
            var result = await _userAuthService.AuthenticateUserAsync(user.Email, "password", CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("token", result);
            _mockUserAuthRepository.Verify(repo => repo.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()), Times.Once);
            _mockPasswordHasherService.Verify(service => service.VerifyPassword(user.Password, "password"), Times.Once);
            _mockTokenService.Verify(service => service.GenerateTokenAsync(user.Id, user.Email, user.Role, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ShouldThrowUnauthorizedAccessException_WhenCredentialsAreInvalid()
        {
            // Arrange
            var user = new Users { Id = 1, Email = "test@example.com", Password = "hashed_password", Role = UserRole.User };
            _mockUserAuthRepository.Setup(repo => repo.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockPasswordHasherService.Setup(service => service.VerifyPassword(user.Password, "invalid_password")).Returns(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userAuthService.AuthenticateUserAsync(user.Email, "invalid_password", CancellationToken.None));
            Assert.Equal("Authentication failed", ex.Message);
            _mockUserAuthRepository.Verify(repo => repo.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>()), Times.Once);
            _mockPasswordHasherService.Verify(service => service.VerifyPassword(user.Password, "invalid_password"), Times.Once);
            _mockTokenService.Verify(service => service.GenerateTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUserByIdAsync_ShouldDeleteUser()
        {
            // Arrange
            int userId = 1;
            _mockUserAuthRepository.Setup(repo => repo.DeleteUserByIdAsync(userId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await _userAuthService.DeleteUserByIdAsync(userId, CancellationToken.None);

            // Assert
            _mockUserAuthRepository.Verify(repo => repo.DeleteUserByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
