using Microsoft.AspNetCore.Mvc;
using RAYS.Services;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using RAYS.ViewModels;  // Add this if MessageViewModel is in the RAYS.ViewModels namespace
using RAYS.Models;
namespace RAYS.Controllers
{
    [Authorize]
    [Route("message")]
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        // Method to get UserID from logged in User
        private int GetLoggedInUserId()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            return userId;
        }

        // GET and POST: message/send/{receiverId}
        [HttpGet("send/{receiverId}")]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(int? receiverId, string newMessage)
        {
            var userId = GetLoggedInUserId();

            // Checking that user is logged in
            if (userId == 0)
            {
                return Unauthorized(); // User is not logged in
            }

            // Checking that RECEIVERID is not null
            if (!receiverId.HasValue)
            {
                ModelState.AddModelError("", "Receiver ID cannot be null.");
                return RedirectToAction("GetMessages", new { senderId = userId });
            }

            // Sending Message via the service layer
            var success = await _messageService.SendMessageAsync(userId, receiverId.Value, newMessage);

            if (!success)
            {
                TempData["ErrorMessage"] = "Message cannot be empty or just whitespace.";
            }

            return RedirectToAction("GetMessages", new { senderId = userId, receiverId = receiverId });
        }

        // GET: message/conversations
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = GetLoggedInUserId();

            // Checking that user is logged in
            if (userId == 0)
            {
                return Unauthorized(); // User is not logged in
            }

            // Getting conversation via the service layer
            var latestMessages = await _messageService.GetConversationsAsync(userId);

            return View(latestMessages);
        }

        // GET: message/{senderId}/{receiverId}
        [HttpGet("{senderId}/{receiverId}")]
        public async Task<IActionResult> GetMessages(int senderId, int receiverId)
        {
            var userId = GetLoggedInUserId();

            // Checking that user is logged in
            if (userId == 0)
            {
                return Unauthorized(); // User is not logged in
            }

            // Checking that user is either sender or receiver
            if (userId != senderId && userId != receiverId)
            {
                return Forbid(); // User trying to access conversation that user is not part of
            }

            int actualSenderId = userId == senderId ? senderId : receiverId;
            int actualReceiverId = userId == senderId ? receiverId : senderId;

            // Getting messages via the service layer
            var viewModel = await _messageService.GetMessagesAsync(actualSenderId, actualReceiverId);


            // Returning viewModel
            return View(viewModel);
        }
    }
}
