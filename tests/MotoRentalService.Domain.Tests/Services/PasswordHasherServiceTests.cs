using MotoRentalService.Domain.Services;

namespace MotoRentalService.Domain.Tests.Services
{
    public class PasswordHasherServiceTests
    {
        private readonly PasswordHasherService _passwordHasherService;

        public PasswordHasherServiceTests()
        {
            _passwordHasherService = new PasswordHasherService();
        }

        [Fact]
        public void HashPassword_ShouldReturnHashedPassword()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            var hashedPassword = _passwordHasherService.HashPassword(password);

            // Assert
            Assert.False(string.IsNullOrEmpty(hashedPassword));
            Assert.True(BCrypt.Net.BCrypt.Verify(password, hashedPassword));
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            // Arrange
            var password = "TestPassword123";
            var hashedPassword = _passwordHasherService.HashPassword(password);

            // Act
            var result = _passwordHasherService.VerifyPassword(hashedPassword, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {
            // Arrange
            var password = "TestPassword123";
            var hashedPassword = _passwordHasherService.HashPassword(password);
            var incorrectPassword = "WrongPassword";

            // Act
            var result = _passwordHasherService.VerifyPassword(hashedPassword, incorrectPassword);

            // Assert
            Assert.False(result);
        }
    }
}
