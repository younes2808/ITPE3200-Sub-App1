using RAYS.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RAYS.DAL;

namespace RAYS.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ServerAPIContext _context;

        public MessageRepository(ServerAPIContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetConversationsAsync(int userId)
        {
            var sentMessages = await _context.Messages
                .Where(m => m.SenderId == userId)
                .ToListAsync();

            var receivedMessages = await _context.Messages
                .Where(m => m.ReceiverId == userId)
                .ToListAsync();

            var allMessages = sentMessages.Concat(receivedMessages)
                .Where(m => m != null);  // Ensure no nulls are included

            return allMessages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.Timestamp).FirstOrDefault())
                .Where(m => m != null)  // Ensure that FirstOrDefault() didn't return null
                .Cast<Message>()  // Cast to Message to remove nullable
                .ToList();
        }




        public async Task<IEnumerable<Message>> GetMessagesAsync(int senderId, int receiverId)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == senderId))
                .ToListAsync();
        }
    }
}
