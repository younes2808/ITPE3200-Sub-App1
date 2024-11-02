using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using System.Threading.Tasks;
using RAYS.ViewModels;
namespace RAYS.Controllers
{
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        // GET: Message/Between/{userId1}/{userId2}
        [HttpGet("Between/{userId1}/{userId2}")]
        public async Task<IActionResult> GetMessagesBetweenUsers(int userId1, int userId2)
        {
            var messages = await _messageService.GetMessagesBetweenUsersAsync(userId1, userId2);
            
             var viewModel = new MessagesViewModel
            {
                Messages = messages,
                MessageRequest = new MessageRequest 
                {
                    SenderId = userId1, // Set the SenderId
                    ReceiverId = userId2, // Set the ReceiverId
                    Content = string.Empty // Initialize Content to an empty string
                }
            };
            return View(viewModel); // Return view with the view model
        }

        // POST: Message/Send
        [HttpPost("Send")]
        public async Task<IActionResult> SendMessage(MessagesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("GetMessagesBetweenUsers", model); // Return to view with validation errors
            }

            await _messageService.SendMessageAsync(model.MessageRequest);
            return RedirectToAction("GetMessagesBetweenUsers", new { userId1 = model.MessageRequest.SenderId, userId2 = model.MessageRequest.ReceiverId });
        }

        // GET: Message/Conversations/{userId}
        [HttpGet("Conversations/{userId}")]
        public async Task<IActionResult> GetConversations(int userId)
        {
            // Fetch the conversation summaries
            var conversations = await _messageService.GetConversationsAsync(userId) ?? Enumerable.Empty<Message>();

            // Map to the ConversationSummaryViewModel
            var conversationSummaries = conversations
                .GroupBy(m => m?.SenderId == userId ? m?.ReceiverId : m?.SenderId) // Check for null in SenderId/ReceiverId
                .Select(g => g?.OrderByDescending(m => m?.Timestamp).FirstOrDefault()) // Null-safe sorting and selection
                .Where(m => m != null) // Ensure no null messages
                .Select(m => new ConversationSummaryViewModel
                {
                    UserId = m?.SenderId == userId ? m?.ReceiverId ?? 0 : m?.SenderId ?? 0, // Null-safe fallback to 0
                    LastMessage = m?.Content ?? string.Empty,
                    LastMessageTimestamp = m?.Timestamp ?? DateTime.MinValue,
                    MessageDirection = m?.SenderId == userId ? "Sent" : "Received"
                })
                .ToList();

            var conversationsViewModel = new ConversationsViewModel
            {
                Conversations = conversationSummaries
            };

            return View("Conversations", conversationsViewModel);
        }

    }
}
