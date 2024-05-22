using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Services;
using MotoRentalService.Domain.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MotoRentalService.Domain.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IOptions<JwtConfig>> _mockJwtConfig;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _jwtConfig = new JwtConfig
            {
                SecretKey = "ThisIsASecretKeyForJwtTokenGener", // Ensure this is at least 32 characters long
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                AccessTokenExpirationMinutes = 60
            };

            _mockJwtConfig = new Mock<IOptions<JwtConfig>>();
            _mockJwtConfig.Setup(config => config.Value).Returns(_jwtConfig);

            _tokenService = new TokenService(_mockJwtConfig.Object);
        }

        [Fact]
        public async Task GenerateTokenAsync_ShouldGenerateToken()
        {
            // Arrange
            int userId = 1;
            string email = "test@example.com";
            UserRole userRole = UserRole.User;
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            string token = await _tokenService.GenerateTokenAsync(userId, email, userRole, cancellationToken);

            // Assert
            Assert.NotNull(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.SecretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidAudience = _jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            Assert.NotNull(jwtToken);
            Assert.Equal(userId.ToString(), jwtToken.Claims.First(x => x.Type == "nameid").Value);
            Assert.Equal(email, jwtToken.Claims.First(x => x.Type == "email").Value);
            Assert.Equal(userRole.ToString(), jwtToken.Claims.First(x => x.Type == "role").Value);
        }

        [Fact]
        public async Task GenerateTokenAsync_ShouldIncludeJtiClaim()
        {
            // Arrange
            int userId = 1;
            string email = "test@example.com";
            UserRole userRole = UserRole.User;

            // Act
            var token = await _tokenService.GenerateTokenAsync(userId, email, userRole, CancellationToken.None);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            Assert.NotNull(jtiClaim);
            Assert.True(Guid.TryParse(jtiClaim.Value, out _));
        }

        [Fact]
        public async Task GenerateTokenAsync_ShouldThrowException_WhenSecretKeyIsInvalid()
        {
            // Arrange
            var invalidJwtConfig = new JwtConfig
            {
                SecretKey = "short",
                AccessTokenExpirationMinutes = 60,
                Issuer = "testissuer",
                Audience = "testaudience"
            };

            var mockInvalidJwtConfig = new Mock<IOptions<JwtConfig>>();
            mockInvalidJwtConfig.Setup(config => config.Value).Returns(invalidJwtConfig);
            var invalidTokenService = new TokenService(mockInvalidJwtConfig.Object);

            int userId = 1;
            string email = "test@example.com";
            UserRole userRole = UserRole.User;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await invalidTokenService.GenerateTokenAsync(userId, email, userRole, CancellationToken.None));
        }

        [Fact]
        public async Task GenerateTokenAsync_ShouldIncludeCorrectExpiration()
        {
            // Arrange
            int userId = 1;
            string email = "test@example.com";
            UserRole userRole = UserRole.User;
            CancellationToken cancellationToken = CancellationToken.None;

            // Act
            string token = await _tokenService.GenerateTokenAsync(userId, email, userRole, cancellationToken);

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.NotNull(jwtToken);
            Assert.True(jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes));
        }
    }
}
