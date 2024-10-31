using RAYS.Models;
using RAYS.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<Message> SendMessageAsync(MessageRequest request)
        {
            _logger.LogInformation("Sending message from User {SenderId} to User {ReceiverId}.", request.SenderId, request.ReceiverId);

            // Validate the request before processing
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                _logger.LogWarning("Attempted to send an empty message from User {SenderId} to User {ReceiverId}.", request.SenderId, request.ReceiverId);
                throw new ArgumentException("Message content cannot be empty.");
            }

            var message = new Message
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                Timestamp = DateTime.UtcNow
            };

            var sentMessage = await _messageRepository.AddAsync(message);
            if (sentMessage == null)
            {
                // Log an error if the message could not be sent for some reason
                _logger.LogError("Failed to send message from User {SenderId} to User {ReceiverId}.", request.SenderId, request.ReceiverId);
                throw new InvalidOperationException("Could not send message.");
            }

            _logger.LogInformation("Message sent successfully with ID {MessageId} from User {SenderId} to User {ReceiverId}.", sentMessage.Id, request.SenderId, request.ReceiverId);
            return sentMessage;
        }

        public async Task<IEnumerable<Message>> GetConversationsAsync(int userId)
        {
            _logger.LogInformation("Retrieving conversations for User {UserId}.", userId);
            var conversations = await _messageRepository.GetConversationsByUserIdAsync(userId);
            _logger.LogInformation("Retrieved {Count} conversations for User {UserId}.", conversations?.Count() ?? 0, userId);
            return conversations ?? Array.Empty<Message>(); // Return an empty array if null
        }

        public async Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(int userId1, int userId2)
        {
            _logger.LogInformation("Retrieving messages between User {UserId1} and User {UserId2}.", userId1, userId2);
            var messages = await _messageRepository.GetMessagesBetweenUsersAsync(userId1, userId2);
            _logger.LogInformation("Retrieved {Count} messages between User {UserId1} and User {UserId2}.", messages?.Count() ?? 0, userId1, userId2);
            return messages ?? Array.Empty<Message>(); // Return an empty array if null
        }
    }
}
