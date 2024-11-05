using RAYS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public interface IUserSearchRepository
    {
        Task<List<User>> SearchUsersAsync(string query);
    }
}
