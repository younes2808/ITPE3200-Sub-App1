using RAYS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public interface IFriendRepository
    {
        Task<bool> SendFriendRequestAsync(Friend friend); 
        Task<IEnumerable<Friend>> GetFriendRequestsAsync(int userId);
        Task<bool> AcceptFriendRequestAsync(int id);
        Task<bool> RejectFriendRequestAsync(int id);
        Task<IEnumerable<Friend>> GetFriendsAsync(int userId);
        Task<bool> DeleteFriendAsync(int userId, int friendId);
    }
}
