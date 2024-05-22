using MotoRentalService.Domain.Entities;
using MotoRentalService.Domain.Interfaces;
using MotoRentalService.Domain.Interfaces.repository;

namespace MotoRentalService.Domain.Services
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserAuthenticationRepository _userAuthRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasherService _passwordHasherService;

        public UserAuthenticationService(IUserAuthenticationRepository userAuthRepository, ITokenService tokenService, IPasswordHasherService passwordHasherService)
        {
            _userAuthRepository = userAuthRepository;
            _tokenService = tokenService;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<string> RegisterUserAsync(Users user, CancellationToken cancellationToken)
        {
            var existingUser = await _userAuthRepository.GetUserByEmailAsync(user.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new Exception("Users is already registerd");
            }
            var hashedPassword = _passwordHasherService.HashPassword(user.Password);
            user.Password = hashedPassword;
            await _userAuthRepository.AddUserAsync(user, cancellationToken);
            return await _tokenService.GenerateTokenAsync(user.Id, user.Email, user.Role, cancellationToken);
        }

        public async Task<string> AuthenticateUserAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userAuthRepository.GetUserByEmailAsync(email, cancellationToken);
            if (user != null && _passwordHasherService.VerifyPassword(user.Password, password))
            {
                return await _tokenService.GenerateTokenAsync(user.Id, user.Email, user.Role, cancellationToken);
            }
            throw new UnauthorizedAccessException("Authentication failed");
        }

        public async Task DeleteUserByIdAsync(int id, CancellationToken cancellationToken)
        {
            await _userAuthRepository.DeleteUserByIdAsync(id, cancellationToken);
        }
    }
}
