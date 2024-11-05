using Microsoft.Extensions.Logging;
using RAYS.Models;
using RAYS.Repositories;
using System.Collections.Generic;
using System.Linq; // Make sure to include this for LINQ methods
using System.Threading.Tasks;

namespace RAYS.Services
{
    public class UserSearchService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserSearchService> _logger;

        public UserSearchService(IUserRepository userRepository, ILogger<UserSearchService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<User>> SearchUsersAsync(string query)
        {
            // Log the incoming query
            _logger.LogInformation("SearchUsersAsync called with query: {Query}", query);

            // Check if the query is null or whitespace
            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Search query is null or empty. Returning an empty result.");
                return new List<User>(); // Return an empty list if the query is invalid
            }

            try
            {
                var users = await _userRepository.SearchUsersAsync(query);

                if (users == null || !users.Any())
                {
                    _logger.LogWarning("No users found for query: {Query}", query);
                    return new List<User>(); // Return an empty list if users is null
                }
                else
                {
                    _logger.LogInformation("{Count} users found for query: {Query}", users.Count(), query);
                }

                return users.ToList(); // Now it is safe to convert to List<User>
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for users with query: {Query}", query);
                throw; // Re-throw the exception after logging
            }
        }
    }
}
