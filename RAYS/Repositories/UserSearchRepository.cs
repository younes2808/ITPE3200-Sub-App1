using RAYS.DAL;
using RAYS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public class UserSearchRepository : IUserSearchRepository
    {
        private readonly ServerAPIContext _context;

        public UserSearchRepository(ServerAPIContext context)
        {
            _context = context;
        }

        public async Task<List<User>> SearchUsersAsync(string query)
        {
            return await _context.Users
                .Where(u => u.Username.Contains(query) || u.Email.Contains(query))
                .ToListAsync();
        }
    }
}
