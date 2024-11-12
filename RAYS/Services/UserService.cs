using RAYS.Models;
using RAYS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace RAYS.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> RegisterUser(string username, string email, string password)
        {
            if (await _userRepository.UsernameOrEmailExistsAsync(username, email))
            {
                _logger.LogWarning("Attempted to register with existing username or email: {Username}, {Email}", username, email);
                throw new System.Exception("Username or email already exists.");
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            await _userRepository.AddUserAsync(user);
            _logger.LogInformation("User registered: {Username}", username);
            return user;
        }

        public async Task<User> LoginUser(string usernameOrEmail, string password)
        {
            try
            {
                // Retrieve the user from the database (could be null if not found)
                var user = await _userRepository.GetUserByUsernameOrEmailAsync(usernameOrEmail);

                if (user == null)
                {
                    // Log the failed login attempt with a warning
                    _logger.LogWarning("Failed login attempt for: {UsernameOrEmail}. User not found.", usernameOrEmail);
                    throw new InvalidOperationException("User not found.");
                }

                // Verify the password with the stored hash
                bool passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

                if (!passwordValid)
                {
                    // Log the failed login attempt due to invalid password
                    _logger.LogWarning("Failed login attempt for: {UsernameOrEmail}. Invalid password.", usernameOrEmail);
                    throw new InvalidOperationException("Invalid credentials.");
                }

                // Log successful login
                _logger.LogInformation("User logged in successfully: {UsernameOrEmail}", usernameOrEmail);

                // Return the user on successful login
                return user;
            }
            catch (Exception ex)
            {
                // Log any unexpected errors with critical error level
                _logger.LogError(ex, "An unexpected error occurred during login for: {UsernameOrEmail}", usernameOrEmail);
                throw new SystemException("An unexpected error occurred. Please try again later.");
            }
        }


        public async Task<User?> GetUserById(int id) // Endret til User? for å indikere at den kan være null
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {Id}", id);
            }
            return user; // Returnerer user, som kan være null
        }

        public async Task<IEnumerable<User>> SearchUsers(string query)
        {
            _logger.LogInformation("Searching users with query: {Query}", query);
            return await _userRepository.SearchUsersAsync(query);
        }
    }
}
