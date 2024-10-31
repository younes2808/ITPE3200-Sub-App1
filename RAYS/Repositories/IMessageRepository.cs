using RAYS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> AddAsync(Message message);
        Task<IEnumerable<Message>> GetConversationsByUserIdAsync(int userId);
        Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(int userId1, int userId2);
    }

}
