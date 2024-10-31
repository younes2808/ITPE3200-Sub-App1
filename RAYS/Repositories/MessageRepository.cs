using RAYS.Models;
using RAYS.DAL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ServerAPIContext _context; 

        public MessageRepository(ServerAPIContext context)
        {
            _context = context;
        }

        public async Task<Message> AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<Message>> GetConversationsByUserIdAsync(int userId)
        {
            // Query to get the last message of each conversation for the user
            var lastMessages = await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.Timestamp).FirstOrDefault())
                .ToListAsync();

            // Filter out null messages and return only non-null messages
            return lastMessages.Where(m => m != null).Cast<Message>();
        }

        public async Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(int userId1, int userId2)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                             (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            // Filter out null messages and return only non-null messages
            return messages.Where(m => m != null).Cast<Message>();
        }
    }
}
