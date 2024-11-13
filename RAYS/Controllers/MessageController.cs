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

        // Hjelpemetode for å hente userId fra den innloggede brukeren
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

            // Sjekk at brukeren er innlogget
            if (userId == 0)
            {
                return Unauthorized(); // Brukeren er ikke logget inn
            }

            // Valider at receiverId ikke er null
            if (!receiverId.HasValue)
            {
                ModelState.AddModelError("", "Receiver ID cannot be null.");
                return RedirectToAction("GetMessages", new { senderId = userId });
            }

            // Send meldingen via service-laget
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

            // Sjekk at brukeren er innlogget
            if (userId == 0)
            {
                return Unauthorized(); // Brukeren er ikke logget inn
            }

            // Hent samtaler via service-laget
            var latestMessages = await _messageService.GetConversationsAsync(userId);

            return View(latestMessages);
        }

        // GET: message/{senderId}/{receiverId}
        [HttpGet("{senderId}/{receiverId}")]
        public async Task<IActionResult> GetMessages(int senderId, int receiverId)
        {
            var userId = GetLoggedInUserId();

            // Sjekk at brukeren er innlogget
            if (userId == 0)
            {
                return Unauthorized(); // Brukeren er ikke logget inn
            }

            // Kontroller at den innloggede brukeren er enten sender eller mottaker
            if (userId != senderId && userId != receiverId)
            {
                return Forbid(); // Den innloggede brukeren prøver å se en samtale de ikke er en del av
            }

            int actualSenderId = userId == senderId ? senderId : receiverId;
            int actualReceiverId = userId == senderId ? receiverId : senderId;

            // Hent meldinger via service-laget
            var viewModel = await _messageService.GetMessagesAsync(actualSenderId, actualReceiverId);


            // Returner viewModel som modellen til visningen
            return View(viewModel);
        }
    }
}
