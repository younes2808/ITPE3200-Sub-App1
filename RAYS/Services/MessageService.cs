using Microsoft.Extensions.Logging;
using RAYS.Models;
using RAYS.Repositories;
using RAYS.ViewModels;
using System;
using System.Threading.Tasks;

namespace RAYS.Services
{
    public class MessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<MessageService> _logger;

        // Bruk IMessageRepository i stedet for MessageRepository
        public MessageService(IMessageRepository messageRepository, ILogger<MessageService> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task<bool> SendMessageAsync(int userId, int receiverId, string newMessage)
        {
            try
            {
                var success = await _messageRepository.SendMessageAsync(userId, receiverId, newMessage);
                if (!success)
                {
                    _logger.LogWarning($"User {userId} attempted to send an empty or whitespace-only message to User {receiverId}.");
                    return false;
                }

                _logger.LogInformation($"User {userId} sent a message to User {receiverId}.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while sending a message from User {userId} to User {receiverId}.");
                return false;
            }
        }

        public async Task<object?> GetConversationsAsync(int userId)
        {
            try
            {
                var conversations = await _messageRepository.GetConversationsAsync(userId);

                if (conversations == null)
                {
                    _logger.LogWarning($"User {userId} failed to fetch conversations.");
                    return null;
                }

                _logger.LogInformation($"User {userId} fetched conversations. Total conversations: {conversations.Count}.");
                return conversations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching conversations for User {userId}.");
                return null;
            }
        }

        public async Task<MessageViewModel?> GetMessagesAsync(int userId, int receiverId)
        {
            try
            {
                // Determine the actual sender and receiver based on who is logged in
                int senderId = userId;  // The logged-in user is the sender
                int actualReceiverId = receiverId;  // The other user is the receiver

                // Fetch messages between the sender and receiver
                var messages = await _messageRepository.GetMessagesAsync(senderId, actualReceiverId);

                // If there are no messages (this is a new conversation), create an empty message list
                if (messages == null || !messages.Any())
                {
                    _logger.LogWarning($"No messages found for User {userId} between Sender {senderId} and Receiver {receiverId}. Starting a new conversation.");
                    messages = new List<Message>();  // Initialize with an empty message list
                }

                // Get usernames for the sender and receiver
                var (senderName, receiverName) = await _messageRepository.GetUserNamesByIdsAsync(senderId, actualReceiverId);

                // Apply default values for null names
                senderName = senderName ?? "Unknown";
                receiverName = receiverName ?? "Unknown";

                // Construct and return the MessageViewModel
                var viewModel = new MessageViewModel
                {
                    SenderId = senderId,
                    ReceiverId = actualReceiverId,
                    Messages = messages,
                    SenderName = senderName,
                    ReceiverName = receiverName,
                    CurrentUserId = userId // Include the logged-in user ID
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching messages between User {userId} and Receiver {receiverId}.");
                return null;
            }
        }



    }
}
