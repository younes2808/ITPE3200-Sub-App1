using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        // GET: Message/Conversations/{userId}
        [HttpGet("Conversations/{userId}")]
        public async Task<IActionResult> GetConversations(int userId)
        {
            var conversations = await _messageService.GetConversationsAsync(userId);
            return View(conversations); // Return a view with the list of conversations
        }

        // GET: Message/Send
        [HttpGet("Send")]
        public IActionResult SendMessage()
        {
            return View(); // Return a view to send a message (form)
        }

        // POST: Message/Send
        [HttpPost("Send")]
        public async Task<IActionResult> SendMessage(MessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request); // Return the view with validation errors
            }

            await _messageService.SendMessageAsync(request);
            return RedirectToAction("GetConversations", new { userId = request.SenderId }); // Redirect after sending the message
        }

        // GET: Message/Between/{userId1}/{userId2}
        [HttpGet("Between/{userId1}/{userId2}")]
        public async Task<IActionResult> GetMessagesBetweenUsers(int userId1, int userId2)
        {
            var messages = await _messageService.GetMessagesBetweenUsersAsync(userId1, userId2);
            return View(messages); // Return a view with messages between two users
        }
    }
}
