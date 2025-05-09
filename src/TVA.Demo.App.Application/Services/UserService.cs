using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Application.Services
{
    public class UserService(IPasswordService passwordService, IUserRepository userRepository, ILogger<UserService> logger) : IUserService
    {
        private readonly IPasswordService _passwordService = passwordService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ILogger<UserService> _logger = logger;

        public async Task<(bool Success, string Message, User User)> RegisterUserAsync(string username, string email, string password, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByUsernameOrEmailAsync(username, cancellationToken) ?? await _userRepository.GetUserByUsernameOrEmailAsync(email, cancellationToken);
            if (existingUser != null)
            {
                if (string.Equals(existingUser.Username, username, StringComparison.OrdinalIgnoreCase))
                    return (false, "Username already exists.", null)!;

                if (string.Equals(existingUser.Email, email, StringComparison.OrdinalIgnoreCase))
                    return (false, "Email already registered.", null)!;
            }

            var passwordHash = _passwordService.HashPassword(password);
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                DateCreated = DateTime.UtcNow,
                IsActive = true
            };

            try
            {
                var createdUser = await _userRepository.CreateUser(user, cancellationToken);
                return (true, "User registered successfully.", createdUser)!;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                _logger.LogError(ex, "Registration failed due to unique constraint violation for username {Username} or email {Email}.", username, email);
                return (false, "Username or Email already exists.", null)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration for {Username}.", username);
                return (false, "An error occurred during registration.", null)!;
            }
        }

        public async Task<User?> AuthenticateUserAsync(string usernameOrEmail, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUsernameOrEmailAsync(usernameOrEmail, cancellationToken);

            if (user != null && user.IsActive && _passwordService.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
    }
}
