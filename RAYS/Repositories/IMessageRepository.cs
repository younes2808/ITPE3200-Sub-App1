using RAYS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public interface IMessageRepository
    {
        Task<bool> SendMessageAsync(int senderId, int receiverId, string content);
        Task<List<Message>> GetMessagesAsync(int senderId, int receiverId);

        // Returnerer en liste av anonyme objekter for samtaler
        Task<List<dynamic>> GetConversationsAsync(int userId);
        Task<string> GetUserNameByIdAsync(int userId);
        // In IMessageRepository
        Task<(string? SenderName, string? ReceiverName)> GetUserNamesByIdsAsync(int senderId, int receiverId);

    }
}
