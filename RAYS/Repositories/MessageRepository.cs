using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RAYS.DAL;
using RAYS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ServerAPIContext _context;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(ServerAPIContext context, ILogger<MessageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(string? SenderName, string? ReceiverName)> GetUserNamesByIdsAsync(int senderId, int receiverId)
        {
            try
            {
                _logger.LogDebug($"Fetching usernames for senderId: {senderId}, receiverId: {receiverId}");
                
                var sender = await _context.Users
                    .Where(u => u.Id == senderId)
                    .Select(u => u.Username)
                    .FirstOrDefaultAsync();

                var receiver = await _context.Users
                    .Where(u => u.Id == receiverId)
                    .Select(u => u.Username)
                    .FirstOrDefaultAsync();

                return (sender, receiver);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Database error while fetching usernames for senderId: {senderId}, receiverId: {receiverId}");
                throw new Exception("Failed to fetch usernames.", ex);
            }
        }

        public async Task<bool> SendMessageAsync(int userId, int receiverId, string newMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newMessage))
                {
                    _logger.LogDebug($"Invalid message content. Skipping message creation for userId: {userId}, receiverId: {receiverId}");
                    return false;
                }

                var message = new Message
                {
                    SenderId = userId,
                    ReceiverId = receiverId,
                    Content = newMessage,
                    Timestamp = DateTime.UtcNow
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                _logger.LogDebug($"Message added to database from userId: {userId} to receiverId: {receiverId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Database error while saving message from userId: {userId} to receiverId: {receiverId}");
                throw new Exception("Failed to send message.", ex);
            }
        }

        public async Task<List<dynamic>> GetConversationsAsync(int userId)
        {
            try
            {
                _logger.LogDebug($"Fetching conversations for userId: {userId}");

                var conversations = await _context.Messages
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .Select(m => new
                    {
                        CorrespondentId = m.SenderId == userId ? m.ReceiverId : m.SenderId,
                        CorrespondentUsername = m.SenderId == userId
                            ? _context.Users.Where(u => u.Id == m.ReceiverId).Select(u => u.Username).FirstOrDefault()
                            : _context.Users.Where(u => u.Id == m.SenderId).Select(u => u.Username).FirstOrDefault(),
                        m.Content,
                        m.Timestamp,
                        IsResponded = m.SenderId != userId
                    })
                    .ToListAsync();

                _logger.LogDebug($"Fetched {conversations.Count} conversations for userId: {userId}");

                var latestMessages = conversations
                    .GroupBy(m => m.CorrespondentId)
                    .Select(g => g.OrderByDescending(m => m.Timestamp).FirstOrDefault())
                    .ToList();

                return latestMessages.Cast<dynamic>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Database error while fetching conversations for userId: {userId}");
                throw new Exception("Failed to fetch conversations.", ex);
            }
        }

        public async Task<List<Message>> GetMessagesAsync(int senderId, int receiverId)
        {
            try
            {
                _logger.LogDebug($"Fetching messages between senderId: {senderId} and receiverId: {receiverId}");

                var messages = await _context.Messages
                    .Where(m =>
                        (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                        (m.SenderId == receiverId && m.ReceiverId == senderId))
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();

                _logger.LogDebug($"Fetched {messages.Count} messages between senderId: {senderId} and receiverId: {receiverId}");

                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Database error while fetching messages between senderId: {senderId} and receiverId: {receiverId}");
                throw new Exception("Failed to fetch messages.", ex);
            }
        }

        public async Task<string> GetUserNameByIdAsync(int userId)
        {
            try
            {
                _logger.LogDebug($"Fetching username for userId: {userId}");

                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.Username)
                    .FirstOrDefaultAsync();

                return user ?? "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Database error while fetching username for userId: {userId}");
                throw new Exception($"Failed to fetch username for userId: {userId}.", ex);
            }
        }
    }
}
