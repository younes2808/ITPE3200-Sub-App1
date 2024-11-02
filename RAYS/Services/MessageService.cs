using RAYS.Models;
using RAYS.Repositories;
using System.Threading.Tasks;

namespace RAYS.Services
{
    public class MessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task SendMessageAsync(MessageRequest request)
        {
            var message = new Message
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                Timestamp = DateTime.UtcNow // Set timestamp as required
            };

            await _messageRepository.AddAsync(message); // Save the message to the repository
        }

        public async Task<IEnumerable<Message>> GetConversationsAsync(int userId)
        {
            return await _messageRepository.GetConversationsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(int userId1, int userId2)
        {
            return await _messageRepository.GetMessagesBetweenUsersAsync(userId1, userId2);
        }
    }
}
