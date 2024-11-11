using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using RAYS.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace RAYS.Controllers
{

    [Authorize]
    public class FriendRequestController : Controller
    {
        private readonly FriendService _friendService;
        private readonly UserService _userService;

        public FriendRequestController(FriendService friendService, UserService userService)
        {
            _friendService = friendService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Fetch friend requests where the current user is the receiver
            var friendRequests = await _friendService.GetFriendRequestsAsync(userId);

            // Map the retrieved friend requests to FriendRequestViewModel, including usernames
            var friendRequestViewModels = new List<FriendRequestViewModel>();

            foreach (var r in friendRequests)
            {
                // Only include requests where the user is the receiver
                if (r.ReceiverId == userId)
                {
                    // Fetch sender and receiver usernames using the UserService
                    var senderUser = await _userService.GetUserById(r.SenderId);
                    var receiverUser = await _userService.GetUserById(r.ReceiverId);

                    friendRequestViewModels.Add(new FriendRequestViewModel
                    {
                        Id = r.Id,  // Friend request ID
                        SenderId = r.SenderId,
                        SenderUsername = senderUser?.Username, // Get the sender's username
                        ReceiverId = r.ReceiverId,
                        ReceiverUsername = receiverUser?.Username, // Get the receiver's username
                        Status = r.Status // Status of the friend request (Pending, Accepted, Rejected)
                    });
                }
            }

            return View(friendRequestViewModels);  // Pass the view models to the view
        }

        // Accept friend request
        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int friendRequestId)  // Ensure the name matches the form's hidden input
        {
            var success = await _friendService.AcceptFriendRequestAsync(friendRequestId);

            if (success)
            {
                // Log success message and redirect
                return RedirectToAction("Index");
            }

            // Handle failure (e.g., the friend request doesn't exist)
            return View("Error");
        }

        // Reject friend request
        [HttpPost]
        public async Task<IActionResult> RejectRequest(int friendRequestId)  // Ensure the name matches the form's hidden input
        {
            var success = await _friendService.RejectFriendRequestAsync(friendRequestId);

            if (success)
            {
                // Log success message and redirect
                return RedirectToAction("Index");
            }

            // Handle failure (e.g., the friend request doesn't exist)
            return View("Error");
        }
        public async Task<IActionResult> FriendsList(int i)
        {
             var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var friends = await _friendService.GetFriendsAsync(userId); // Fetch friends dynamically
            ViewData["FriendsList"] = friends; // Set ViewData to pass to the partial
            return View();
        }

    }
}
