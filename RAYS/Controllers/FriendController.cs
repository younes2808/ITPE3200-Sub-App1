using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using RAYS.ViewModels; // Husk å inkludere dette namespace for View Models
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RAYS.Controllers
{
    [Route("Friend")]
    [Authorize]
    public class FriendController : Controller
    {
        private readonly FriendService _friendService;

        public FriendController(FriendService friendService)
        {
            _friendService = friendService;
        }

        // Send friend request view
        [HttpGet("RequestForm")]
        public IActionResult RequestForm()
        {
            return View(); // Viser skjemaet for å sende en venneforespørsel
        }

        // POST: /Friend/Request
        [HttpPost("Request")]
        public async Task<IActionResult> SendFriendRequest([FromForm] Friend friendRequest)
        {
            if (await _friendService.SendFriendRequestAsync(friendRequest))
            {
                TempData["SuccessMessage"] = "Friend request sent successfully.";
                return RedirectToAction("Requests", new { userId = friendRequest.SenderId });
            }
            TempData["ErrorMessage"] = "Failed to send friend request.";
            return View("RequestForm");
        }

        // GET: /Friend/Requests/{userId}
        [HttpGet("Requests/{userId}")]
        public async Task<IActionResult> GetFriendRequests(int userId)
        {
            var requests = await _friendService.GetFriendRequestsAsync(userId);

            // Map the requests to the FriendRequestViewModel
            var requestViewModels = requests.Select(request => new FriendRequestViewModel
            {
                Id = request.Id,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Status = request.Status
            }).ToList();

            return View("FriendRequests", requestViewModels);
        }

        // Accept friend request
        [HttpPost("Accept/{id}")]
        public async Task<IActionResult> AcceptFriendRequest(int id)
        {
            if (await _friendService.AcceptFriendRequestAsync(id))
            {
                TempData["SuccessMessage"] = "Friend request accepted.";
                return RedirectToAction("Requests", new { userId = id });
            }
            TempData["ErrorMessage"] = "Friend request not found.";
            return RedirectToAction("Requests", new { userId = id });
        }

        // Reject friend request
        [HttpPost("Reject/{id}")]
        public async Task<IActionResult> RejectFriendRequest(int id)
        {
            if (await _friendService.RejectFriendRequestAsync(id))
            {
                TempData["SuccessMessage"] = "Friend request rejected.";
                return RedirectToAction("Requests", new { userId = id });
            }
            TempData["ErrorMessage"] = "Friend request not found.";
            return RedirectToAction("Requests", new { userId = id });
        }

        // GET: /Friend/List/{userId}
        [HttpGet("List/{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            var friends = await _friendService.GetFriendsAsync(userId);
            var friendViewModels = friends.Select(friend => new FriendViewModel
            {
                FriendId = friend.ReceiverId == userId ? friend.SenderId : friend.ReceiverId,
                FriendName = "Friend's Name" // Erstatt med faktisk logikk for navn hvis nødvendig
            }).ToList();

            var friendListViewModel = new FriendListViewModel
            {
                Friends = friendViewModels
            };

            return View("FriendsList", friendListViewModel);
        }

        // DELETE: /Friend/Delete/{userId}/{friendId}
        [HttpPost("Delete/{userId}/{friendId}")]
        public async Task<IActionResult> DeleteFriend(int userId, int friendId)
        {
            if (await _friendService.DeleteFriendAsync(userId, friendId))
            {
                TempData["SuccessMessage"] = "Friend deleted successfully.";
                return RedirectToAction("List", new { userId });
            }
            TempData["ErrorMessage"] = "Friend not found.";
            return RedirectToAction("List", new { userId });
        }
    }
}
