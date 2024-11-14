using Microsoft.Extensions.Logging;
using RAYS.Models;
using RAYS.Repositories;
using RAYS.ViewModels;
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

        // SendMessageAsync
        public async Task<bool> SendMessageAsync(int userId, int receiverId, string newMessage)
        {
            // _logger.LogInformation($"Starting SendMessageAsync with userId: {userId}, receiverId: {receiverId}");

            // Inputvalidering
            if (userId <= 0 || receiverId <= 0)
            {
                _logger.LogWarning($"Invalid userId or receiverId. userId: {userId}, receiverId: {receiverId}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(newMessage))
            {
                _logger.LogWarning($"Message content is empty. userId: {userId}, receiverId: {receiverId}");
                return false;
            }

            try
            {
                // Kall til repository, ingen logging av DB-operasjoner her
                var success = await _messageRepository.SendMessageAsync(userId, receiverId, newMessage);

                // Logge resultatet av forretningslogikk
                _logger.LogInformation($"SendMessageAsync completed with status: {(success ? "Success" : "Failure")}, message sent from userId: {userId} to receiverId: {receiverId}");
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in SendMessageAsync for userId {userId} to receiverId {receiverId}");
                return false;
            }
        }

        // GetConversationsAsync
        public async Task<object?> GetConversationsAsync(int userId)
        {
           // _logger.LogInformation($"Starting GetConversationsAsync for userId: {userId}");

            // Inputvalidering
            if (userId <= 0)
            {
                _logger.LogWarning($"Invalid userId: {userId} in GetConversationsAsync.");
                return null;
            }

            try
            {
                // Henter samtaler, overlater logging av DB til repository
                var conversations = await _messageRepository.GetConversationsAsync(userId);

                if (conversations == null || conversations.Count == 0)
                {
                    _logger.LogInformation($"No conversations found for userId: {userId}");
                }
                else
                {
                    _logger.LogInformation($"Conversations fetched successfully for userId: {userId}, count: {conversations.Count}");
                }

                return conversations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in GetConversationsAsync for userId: {userId}");
                return null;
            }
        }

        // GetMessagesAsync
        public async Task<MessageViewModel?> GetMessagesAsync(int userId, int receiverId)
        {
           // _logger.LogInformation($"Starting GetMessagesAsync with userId: {userId}, receiverId: {receiverId}");

            // Inputvalidering
            if (userId <= 0 || receiverId <= 0)
            {
                _logger.LogWarning($"Invalid userId or receiverId. userId: {userId}, receiverId: {receiverId}");
                return null;
            }

            try
            {
                // Henter meldinger uten å logge databaseoperasjoner
                var messages = await _messageRepository.GetMessagesAsync(userId, receiverId);

                // Sjekk om meldinger ble funnet
                if (messages == null || messages.Count == 0)
                {
                    _logger.LogInformation($"No messages found between userId: {userId} and receiverId: {receiverId}");
                    messages = new List<Message>();
                }

                // Henter brukernavn fra repository
                var (senderName, receiverName) = await _messageRepository.GetUserNamesByIdsAsync(userId, receiverId);

                // Lag MessageViewModel
                var viewModel = new MessageViewModel
                {
                    SenderId = userId,
                    ReceiverId = receiverId,
                    Messages = messages,
                    SenderName = senderName ?? "Unknown",
                    ReceiverName = receiverName ?? "Unknown",
                    CurrentUserId = userId
                };

                _logger.LogInformation($"MessageViewModel created successfully for userId: {userId}, receiverId: {receiverId}");
                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in GetMessagesAsync for userId: {userId} and receiverId: {receiverId}");
                return null;
            }
        }
    }
}
