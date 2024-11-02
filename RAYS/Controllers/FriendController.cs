using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using RAYS.ViewModels; // Make sure to include this namespace for your view models
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : Controller
    {
        private readonly FriendService _friendService;

        public FriendController(FriendService friendService)
        {
            _friendService = friendService;
        }

        // POST: api/friend/request
        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] Friend friendRequest)
        {
            if (await _friendService.SendFriendRequestAsync(friendRequest))
            {
                return Ok("Friend request sent successfully.");
            }
            return BadRequest("Failed to send friend request.");
        }

        // GET: api/friend/requests/{userId}
        [HttpGet("requests/{userId}")]
        public async Task<IActionResult> GetFriendRequests(int userId)
        {
            // Fetch friend requests using the service
            var requests = await _friendService.GetFriendRequestsAsync(userId);

            // Map the requests to the FriendRequestViewModel
            var requestViewModels = requests.Select(request => new FriendRequestViewModel
            {
                Id = request.Id,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Status = request.Status
            }).ToList();

            // Return the list of requests as a JSON response
            return Ok(requestViewModels); // or return View("FriendRequests", requestViewModels) if you are using a view
        }

        // PUT: api/friend/accept/{id}
        [HttpPut("accept/{id}")]
        public async Task<IActionResult> AcceptFriendRequest(int id)
        {
            if (await _friendService.AcceptFriendRequestAsync(id))
            {
                return NoContent(); // Return no content on successful acceptance
            }
            return NotFound("Friend request not found.");
        }

        // PUT: api/friend/reject/{id}
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectFriendRequest(int id)
        {
            if (await _friendService.RejectFriendRequestAsync(id))
            {
                return NoContent(); // Return no content on successful rejection
            }
            return NotFound("Friend request not found.");
        }

        // GET: api/friend/list/{userId}
        [HttpGet("list/{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            var friends = await _friendService.GetFriendsAsync(userId);
            var friendViewModels = friends.Select(friend => new FriendViewModel
            {
                FriendId = friend.ReceiverId == userId ? friend.SenderId : friend.ReceiverId,
                FriendName = "Get Friend's Name Here" // You would replace this with actual name fetching logic
            }).ToList();

            var friendListViewModel = new FriendListViewModel
            {
                Friends = friendViewModels
            };

            return Ok(friendListViewModel); // or return View("FriendsList", friendListViewModel) if you are using a view
        }

        // DELETE: api/friend/delete/{userId}/{friendId}
        [HttpDelete("delete/{userId}/{friendId}")]
        public async Task<IActionResult> DeleteFriend(int userId, int friendId)
        {
            if (await _friendService.DeleteFriendAsync(userId, friendId))
            {
                return NoContent(); // Return no content on successful deletion
            }
            return NotFound("Friend not found.");
        }
    }
}
