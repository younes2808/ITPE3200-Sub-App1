using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RAYS.DAL;
using RAYS.Models;
using RAYS.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace RAYS.Controllers
{
    [Authorize]
    [Route("message")]
    public class MessageController : Controller
    {
        private readonly ServerAPIContext _context;

        public MessageController(ServerAPIContext context)
        {
            _context = context;
        }

        // GET and POST: message/send/{receiverId}
        [HttpGet("send/{receiverId}")]
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(int? receiverId, string newMessage)
        {
            // Hent UserId fra den innloggede brukeren
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Hvis userId er 0 (ikke logget inn), skal de ikke kunne sende meldinger
            if (userId == 0)
            {
                return Unauthorized(); // Brukeren er ikke logget inn
            }

            // Sjekk om meldingen er tom eller bare består av hvite mellomrom
            // Sjekk om meldingen er tom eller bare består av hvite mellomrom
            if (string.IsNullOrWhiteSpace(newMessage))
            {
                TempData["ErrorMessage"] = "Message cannot be empty or just whitespace."; // Sett feilmeldingen i TempData
                return RedirectToAction("GetMessages", new { senderId = userId, receiverId = receiverId });
            }


            // Null check before accessing receiverId
            if (!receiverId.HasValue)
            {
                ModelState.AddModelError("", "Receiver ID cannot be null.");
                return RedirectToAction("GetMessages", new { senderId = userId });
            }

            // Lag den nye meldingen
            var message = new Message
            {
                SenderId = userId, // Bruk den innloggede brukerens ID som sender
                ReceiverId = receiverId.Value, // Now safe to access .Value
                Content = newMessage,
                Timestamp = DateTime.UtcNow
            };

            // Legg til meldingen i databasen og lagre
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Redirect til samtalen etter at meldingen er sendt
            return RedirectToAction("GetMessages", new { senderId = userId, receiverId = receiverId });
        }


        // GET: message/conversations
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Hvis userId er 0 (ikke logget inn), skal de ikke kunne se samtaler
            if (userId == 0)
            {
                return Unauthorized(); // Brukeren er ikke logget inn
            }

            // Hent alle meldinger der brukeren er sender eller mottaker
            var conversations = await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => new
                {
                    CorrespondentId = m.SenderId == userId ? m.ReceiverId : m.SenderId, // Finn ID for samtalepartner
                    m.Content,
                    m.Timestamp,
                    IsResponded = m.SenderId != userId // Sjekk om brukeren var mottaker av den siste meldingen
                })
                .ToListAsync();

            // Gruppér på samtalepartner og ta kun den nyeste meldingen i hver samtale
            var latestMessages = conversations
                .GroupBy(m => m.CorrespondentId)
                .Select(g => g.OrderByDescending(m => m.Timestamp).FirstOrDefault())
                .ToList();

            return View(latestMessages);
        }

        [HttpGet("{senderId}/{receiverId}")]
        public async Task<IActionResult> GetMessages(int senderId, int receiverId)
        {
            // Hent UserId fra den innloggede brukeren
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Hvis userId er 0 (ikke logget inn), skal de ikke kunne se meldinger
            if (userId == 0)
            {
                return Unauthorized(); // Brukeren er ikke logget inn
            }

            // Kontroller at den innloggede brukeren er enten sender eller mottaker
            if (userId != senderId && userId != receiverId)
            {
                return Forbid(); // Den innloggede brukeren prøver å se en samtale de ikke er en del av
            }

            // Hent meldinger mellom de to brukerne, uavhengig av hvem som er sender/mottaker
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.Timestamp) // Sorter meldinger etter tid
                .ToListAsync();

            // Finn riktig sender og mottaker basert på den innloggede brukeren
            int actualSenderId = userId == senderId ? senderId : receiverId;
            int actualReceiverId = userId == senderId ? receiverId : senderId;

            // Hent brukernavn for sender og mottaker
            var senderName = await _context.Users
                .Where(u => u.Id == actualSenderId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            var receiverName = await _context.Users
                .Where(u => u.Id == actualReceiverId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            // Apply default values for null names
            senderName = senderName ?? "Unknown";  // Default to "Unknown" if senderName is null
            receiverName = receiverName ?? "Unknown";  // Default to "Unknown" if receiverName is null

            // Opprett MessageViewModel med nødvendige data
            var viewModel = new MessageViewModel
            {
                SenderId = actualSenderId,
                ReceiverId = actualReceiverId,
                Messages = messages,  // En liste av Message-objektene
                SenderName = senderName,
                ReceiverName = receiverName,
                CurrentUserId = userId // Send med CurrentUserId for visning i View
            };

            return View(viewModel); // Returner viewModel som modellen til visningen
        }

    }
}
