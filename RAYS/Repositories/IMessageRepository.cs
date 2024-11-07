using RAYS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetConversationsAsync(int userId);
        Task<IEnumerable<Message>> GetMessagesAsync(int senderId, int receiverId);
    }
}
