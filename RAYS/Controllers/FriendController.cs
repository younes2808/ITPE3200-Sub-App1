using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using System.Collections.Generic;
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
            var requests = await _friendService.GetFriendRequestsAsync(userId);
            return View("FriendRequests", requests); // Return a view for friend requests
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
            return View("FriendsList", friends); // Return a view for the friends list
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
