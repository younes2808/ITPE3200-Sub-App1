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
        private readonly ILogger<MessageRepository> _logger;  // Add logger

        public MessageRepository(ServerAPIContext context, ILogger<MessageRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(string? SenderName, string? ReceiverName)> GetUserNamesByIdsAsync(int senderId, int receiverId)
        {
            try
            {
                // Fetch the sender and receiver usernames from the database
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
                _logger.LogError(ex, $"Error fetching usernames for senderId: {senderId} and receiverId: {receiverId}");
                throw new Exception("Failed to fetch usernames.", ex); // Throw custom exception with context
            }
        }

        public async Task<bool> SendMessageAsync(int userId, int receiverId, string newMessage)
        {
            try
            {
                // Validate the message
                if (string.IsNullOrWhiteSpace(newMessage))
                {
                    _logger.LogWarning($"Message is empty or contains only whitespace. userId: {userId}, receiverId: {receiverId}");
                    return false;  // Invalid message
                }

                // Create the new message
                var message = new Message
                {
                    SenderId = userId,
                    ReceiverId = receiverId,
                    Content = newMessage,
                    Timestamp = DateTime.UtcNow
                };

                // Add to the database and save
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Message sent from user {userId} to user {receiverId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message from user {userId} to user {receiverId}");
                throw new Exception("Failed to send message.", ex);  // Throw custom exception
            }
        }

        public async Task<List<dynamic>> GetConversationsAsync(int userId)
        {
            try
            {
                // Fetch conversations where the user is either the sender or the receiver
                var conversations = await _context.Messages
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .Select(m => new
                    {
                        CorrespondentId = m.SenderId == userId ? m.ReceiverId : m.SenderId,
                        // Dynamically fetch the correspondent's username based on who is the sender or receiver
                        CorrespondentUsername = m.SenderId == userId
                            ? _context.Users.Where(u => u.Id == m.ReceiverId).Select(u => u.Username).FirstOrDefault()
                            : _context.Users.Where(u => u.Id == m.SenderId).Select(u => u.Username).FirstOrDefault(),
                        m.Content,
                        m.Timestamp,
                        IsResponded = m.SenderId != userId
                    })
                    .ToListAsync();

                if (!conversations.Any())
                {
                    _logger.LogInformation($"No conversations found for user {userId}.");
                    return new List<dynamic>();  // Return empty list if no conversations exist
                }

                // Group by correspondent and select the most recent message per conversation
                var latestMessages = conversations
                    .GroupBy(m => m.CorrespondentId)
                    .Select(g => g.OrderByDescending(m => m.Timestamp).FirstOrDefault())
                    .ToList();

                return latestMessages.Cast<dynamic>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching conversations for user {userId}");
                throw new Exception("Failed to fetch conversations.", ex);  // Throw custom exception
            }
        }


        public async Task<List<Message>> GetMessagesAsync(int senderId, int receiverId)
        {
            try
            {
                // Fetch messages between two users, regardless of who is sender or receiver
                var messages = await _context.Messages
                    .Where(m =>
                        (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                        (m.SenderId == receiverId && m.ReceiverId == senderId))
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();

                if (!messages.Any())
                {
                    _logger.LogInformation($"No messages found between user {senderId} and user {receiverId}.");
                    return new List<Message>();  // Return empty list if no messages exist
                }

                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching messages between user {senderId} and user {receiverId}");
                throw new Exception("Failed to fetch messages.", ex);  // Throw custom exception
            }
        }

        public async Task<string> GetUserNameByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.Username)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return "Unknown"; // Default return value
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching username for userId: {userId}");
                throw new Exception($"Failed to fetch username for userId: {userId}.", ex);  // Throw custom exception
            }
        }
    }
}
