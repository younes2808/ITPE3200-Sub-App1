using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RAYS.Models;
using RAYS.Repositories;

namespace RAYS.Services
{
    public class MessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessageService> _logger;

        public MessageService(IMessageRepository messageRepository, ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task AddMessageAsync(Message message)
        {
            _logger.LogInformation("Adding a new message from {SenderId} to {ReceiverId}.", message.SenderId, message.ReceiverId);

            try
            {
                await _messageRepository.AddMessageAsync(message);
                _logger.LogInformation("Message successfully added.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a message from {SenderId} to {ReceiverId}.", message.SenderId, message.ReceiverId);
                throw; // Rethrow exception after logging it
            }
        }

        public async Task<IEnumerable<Message>> GetConversationsAsync(int userId)
        {
            _logger.LogInformation("Fetching conversations for user {UserId}.", userId);

            try
            {
                var conversations = await _messageRepository.GetConversationsAsync(userId);
                _logger.LogInformation("Successfully retrieved conversations for user {UserId}.", userId);
                return conversations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching conversations for user {UserId}.", userId);
                throw;
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(int senderId, int receiverId)
        {
            _logger.LogInformation("Fetching messages between {SenderId} and {ReceiverId}.", senderId, receiverId);

            try
            {
                var messages = await _messageRepository.GetMessagesAsync(senderId, receiverId);
                _logger.LogInformation("Successfully retrieved messages between {SenderId} and {ReceiverId}.", senderId, receiverId);
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching messages between {SenderId} and {ReceiverId}.", senderId, receiverId);

                // Return an empty list instead of failing silently or returning nothing
                return new List<Message>();
            }
        }
    }
}
