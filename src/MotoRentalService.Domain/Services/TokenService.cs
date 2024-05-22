using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MotoRentalService.Domain.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly SymmetricSecurityKey _signingKey;

        public TokenService(IOptions<JwtConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        }

        public async Task<string> GenerateTokenAsync(int userId, string email, UserRole userRole, CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, userRole.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes),
                SigningCredentials = credentials,
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
