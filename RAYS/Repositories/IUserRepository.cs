using RAYS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UsernameOrEmailExistsAsync(string username, string email);
        Task AddUserAsync(User user);
        Task<IEnumerable<User>> SearchUsersAsync(string query);
    }
}
