using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RAYS.DAL;
using RAYS.Models;
using RAYS.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Route("message")]
    public class MessageController : Controller
    {
        private readonly ServerAPIContext _context;

        public MessageController(ServerAPIContext context)
        {
            _context = context;
        }

        // GET: message/send
        [HttpGet("send")]
        public IActionResult SendMessageView()
        {
            // Return a view for sending a message
            return View();
        }

        // POST: message/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest messageRequest)
        {
            // Check if the message request is null
            if (messageRequest == null)
            {
                ModelState.AddModelError("", "Message cannot be null");
                return View("SendMessageView", messageRequest);
            }

            // Validate that SenderId and ReceiverId are not the same
            if (messageRequest.SenderId == messageRequest.ReceiverId)
            {
                ModelState.AddModelError("", "Sender and receiver cannot be the same user.");
                return View("SendMessageView", messageRequest);
            }

            // Create a new Message instance from the MessageRequest
            var message = new Message
            {
                SenderId = messageRequest.SenderId,
                ReceiverId = messageRequest.ReceiverId,
                Content = messageRequest.Content,
                Timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo")),
            };

            // Add the new message
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Redirect to a view showing the conversation
            return RedirectToAction(nameof(GetMessages), new { senderId = message.SenderId, receiverId = message.ReceiverId });
        }

        // GET: message/conversations/{userId}
        [HttpGet("conversations/{userId}")]
        public async Task<IActionResult> GetConversations(int userId)
        {
            // Get all sent messages
            var sentMessages = await _context.Messages
                .Where(m => m.SenderId == userId)
                .Select(m => new
                {
                    UserId = m.ReceiverId,
                    m.Content,
                    m.Timestamp,
                    IsResponded = true
                })
                .ToListAsync();

            // Get all received messages
            var receivedMessages = await _context.Messages
                .Where(m => m.ReceiverId == userId)
                .Select(m => new
                {
                    UserId = m.SenderId,
                    m.Content,
                    m.Timestamp,
                    IsResponded = false
                })
                .ToListAsync();

            // Combine both lists and select the latest message for each user
            var allMessages = sentMessages
                .Concat(receivedMessages)
                .GroupBy(m => m.UserId)
                .Select(g => g.OrderByDescending(m => m.Timestamp).FirstOrDefault())
                .ToList();

            // Return the view with the messages
            return View(allMessages);
        }

        // GET: message/{senderId}/{receiverId}
        [HttpGet("{senderId}/{receiverId}")]
        public async Task<IActionResult> GetMessages(int senderId, int receiverId)
        {
            // Hent meldinger mellom sender og mottaker
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId))
                .ToListAsync();

            // Hent brukernavn for sender og receiver
            var senderName = await _context.Users
                .Where(u => u.Id == senderId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            var receiverName = await _context.Users
                .Where(u => u.Id == receiverId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();

            // Opprett MessageViewModel med nødvendige data
            var viewModel = new MessageViewModel
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Messages = messages,  // En liste av Message-objektene
                SenderName = senderName,
                ReceiverName = receiverName
            };

            return View(viewModel); // Returner viewModel som modellen til visningen
        }

    }
}
