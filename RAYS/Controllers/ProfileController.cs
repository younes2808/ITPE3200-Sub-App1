using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RAYS.Services;
using RAYS.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using RAYS.Models;
using Microsoft.Extensions.Logging;

namespace RAYS.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly PostService _postService;
        private readonly UserService _userService;
        private readonly FriendService _friendService;
        private readonly LikeService _likeService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(PostService postService, UserService userService, FriendService friendService, LikeService likeService, ILogger<ProfileController> logger)
        {
            _postService = postService;
            _userService = userService;
            _friendService = friendService;
            _likeService = likeService;
            _logger = logger;
        }

        public async Task<IActionResult> Profile(int userId, string viewType = "Posts")
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found for userId: {UserId}", userId);
                return NotFound();
            }

            ViewBag.ViewType = viewType;

            // Get all relevant friend data
            var allFriendRequests = await _friendService.GetFriendRequestsAsync(currentUserId);
            var friends = await GetFriends(userId);  // Get the friends of the user being viewed

            // Check if the logged-in user and the viewed user are already friends
            var isFriend = friends.Any(f => f.UserId == currentUserId);

            // Check if there is any pending request between these two users (either direction)
            var hasPendingRequest = allFriendRequests.Any(r =>
                (r.SenderId == currentUserId && r.ReceiverId == userId && r.Status == "Pending") ||
                (r.SenderId == userId && r.ReceiverId == currentUserId && r.Status == "Pending")
            );

            var incomingRequests = allFriendRequests
                .Where(r => r.ReceiverId == currentUserId && r.SenderId == userId && r.Status == "Pending");

            // The view model will include:
            var userViewModel = new UserViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                Posts = await GetPostsForUser(userId),
                LikedPosts = await GetLikedPostsForUser(userId),
                Friends = friends,  // Now using the friends of the user being viewed
                FriendRequests = incomingRequests,
                IsFriend = isFriend,
                HasPendingRequest = hasPendingRequest
            };

            // Now check if the user can send a friend request or not
            // If the user is already friends, don't show the friend request button
            if (isFriend)
            {
                userViewModel.HasPendingRequest = false;  // Ensure no pending request button appears
            }
            else if (hasPendingRequest)
            {
                userViewModel.HasPendingRequest = true;  // Pending request logic
            }

            return View(userViewModel);
        }




        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int receiverId)
        {
            var senderId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var friendRequest = new Friend { SenderId = senderId, ReceiverId = receiverId, Status = "Pending" };

            await _friendService.SendFriendRequestAsync(friendRequest);
            return RedirectToAction("Profile", new { userId = receiverId });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(int requestId)
        {
            // Retrieve the current user's ID
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Get all friend requests for the current user
            var friendRequests = await _friendService.GetFriendRequestsAsync(currentUserId);

            // Find the friend request by its ID
            var friendRequest = friendRequests.FirstOrDefault(r => r.Id == requestId);
            if (friendRequest == null)
            {
                _logger.LogWarning("Friend request with ID {RequestId} not found for UserId {CurrentUserId}", requestId, currentUserId);
                return NotFound(); // If the request does not exist, handle accordingly
            }

            // Accept the friend request
            await _friendService.AcceptFriendRequestAsync(requestId);
            _logger.LogInformation("UserId {CurrentUserId} accepted friend request from UserId {SenderId}", currentUserId, friendRequest.SenderId);


            // Redirect to the sender's profile (the one who sent the request)
            return RedirectToAction("Profile", new { userId = friendRequest.SenderId });
        }

        [HttpPost]
        public async Task<IActionResult> RejectFriendRequest(int requestId)
        {
            // Retrieve the current user's ID
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Get all friend requests for the current user
            var friendRequests = await _friendService.GetFriendRequestsAsync(currentUserId);

            // Find the friend request by its ID
            var friendRequest = friendRequests.FirstOrDefault(r => r.Id == requestId);
            if (friendRequest == null)
            {
                _logger.LogWarning("Friend request with ID {RequestId} not found for UserId {CurrentUserId}", requestId, currentUserId);
                return NotFound(); // If the request does not exist, handle accordingly
            }

            // Reject the friend request
            await _friendService.RejectFriendRequestAsync(requestId);
            _logger.LogInformation("UserId {CurrentUserId} rejected friend request from UserId {SenderId}", currentUserId, friendRequest.SenderId);

            // Redirect to the sender's profile (the one who sent the request)
            return RedirectToAction("Profile", new { userId = friendRequest.SenderId });
        }

        // Controller action to delete a friend
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(int friendId)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Call the DeleteFriendAsync method from the FriendService to remove the friendship
            var result = await _friendService.DeleteFriendAsync(currentUserId, friendId);

            if (result)
            {
                TempData["Message"] = "Friend deleted successfully!";
            }
            else
            {
                _logger.LogError("Failed to delete friend UserId {FriendId} for UserId {CurrentUserId}", friendId, currentUserId);
                TempData["Message"] = "Failed to delete friend.";
            }

            // After deleting, redirect to the profile page of the user being viewed
            return RedirectToAction("Profile", new { userId = friendId });
        }


        private async Task<List<PostViewModel>> GetPostsForUser(int userId)
        {
            var posts = await _postService.GetByUserIdAsync(userId);
            var postViewModels = new List<PostViewModel>();
            // Retrieve the current user's ID
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            foreach (var post in posts)
            {
                // Fetch the user associated with the post
                var user = await _userService.GetUserById(post.UserId);

                // Add PostViewModel to the list with the user information
                postViewModels.Add(new PostViewModel
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImagePath = post.ImagePath,
                    VideoUrl = post.VideoUrl,
                    Location = post.Location,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    Username = user?.Username ?? "Unknown",  // Use "Unknown" if user is null
                    LikeCount = await _likeService.GetLikesForPostAsync(post.Id),
                    IsLikedByUser = await _postService.IsPostLikedByUserAsync(currentUserId, post.Id)
                });
            }

            return postViewModels;
        }


        private async Task<List<PostViewModel>> GetLikedPostsForUser(int userId)
        {
            var likedPosts = await _postService.GetLikedPostsByUserIdAsync(userId);
            var likedPostViewModels = new List<PostViewModel>();

            // Retrieve the current user's ID (the viewer, e.g., User 1)
            var currentUserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            foreach (var post in likedPosts)
            {
                // Fetch the user associated with the post
                var user = await _userService.GetUserById(post.UserId);

                // Add PostViewModel to the list with the user information
                likedPostViewModels.Add(new PostViewModel
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImagePath = post.ImagePath,
                    VideoUrl = post.VideoUrl,
                    Location = post.Location,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    Username = user?.Username ?? "Unknown",  // Use "Unknown" if user is null
                    LikeCount = await _likeService.GetLikesForPostAsync(post.Id),

                    // Use currentUserId (the viewer's ID) to determine if they have liked the post
                    IsLikedByUser = await _postService.IsPostLikedByUserAsync(currentUserId, post.Id)
                });
            }

            return likedPostViewModels;
        }



        private async Task<List<UserViewModel>> GetFriends(int userId)
        {
            // Fetch the list of friends for the current user
            var friends = await _friendService.GetFriendsAsync(userId);

            var friendViewModels = new List<UserViewModel>();

            // Map each friend (either as Sender or Receiver) to a UserViewModel
            foreach (var friend in friends)
            {
                // Get the friend's user ID (exclude the current user)
                var friendId = friend.ReceiverId == userId ? friend.SenderId : friend.ReceiverId;

                // Ensure we don't include the current user in the list of friends
                if (friendId != userId)
                {
                    // Retrieve the actual user details for this friend (e.g., Username)
                    var user = await _userService.GetUserById(friendId);  // Assuming GetUserById fetches the user by their Id

                    if (user != null)
                    {
                        friendViewModels.Add(new UserViewModel
                        {
                            UserId = user.Id,
                            Username = user.Username
                        });
                    }
                }
            }

            return friendViewModels;
        }

        [HttpPost]
        public async Task<IActionResult> Like(int postId, string viewType, int ViewId)
        {
            //Getting UserID
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            //If unliked, like else unlike
            if (!await _postService.IsPostLikedByUserAsync(userId, postId))
            {
                await _postService.LikePostAsync(userId, postId);
                _logger.LogInformation("UserId {UserId} liked post {PostId}", userId, postId);
            }
            else
            {
                await _postService.UnlikePostAsync(userId, postId);
                _logger.LogInformation("UserId {UserId} unliked post {PostId}", userId, postId);
            }

            return RedirectToAction("Profile", new { userId = ViewId, viewType = viewType });
        }

        [HttpPost]
        public async Task<IActionResult> Unlike(int postId, string viewType, int ViewId)
        {
            //Getting UserID
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            //If liked, unlike else like
            if (await _postService.IsPostLikedByUserAsync(userId, postId))
            {
                await _postService.UnlikePostAsync(userId, postId);
                _logger.LogInformation("UserId {UserId} unliked post {PostId}", userId, postId);
            }
            else
            {
                await _postService.LikePostAsync(userId, postId);
                _logger.LogInformation("UserId {UserId} liked post {PostId}", userId, postId);
            }

            return RedirectToAction("Profile", new { userId = ViewId, viewType = viewType });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(PostViewModel model, string viewType, int ViewId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            //Checking CREDENTIALS
            if (model.UserId != userId)
            {
                _logger.LogWarning("UserId {UserId} attempted to update another user's post {PostId}", userId, model.Id);
                return Forbid();
            }

            // Check if content is empty
            if (string.IsNullOrEmpty(model.Content))
            {
                TempData["ErrorPostId"] = model.Id;  // Store post ID for error display
                TempData["ContentErrorMessage"] = "Updated content cannot be empty."; // Store error message
                return RedirectToAction("Profile", new { userId = userId, viewType = viewType });
            }

            var post = await _postService.GetByIdAsync(model.Id);
            if (post == null)
            {
                _logger.LogWarning("Post with ID {PostId} not found for update by UserId {UserId}", model.Id, userId);
                return NotFound();
            }
            post.Content = model.Content;
            post.VideoUrl = model.VideoUrl;
            post.Location = model.Location;
            //Updating post
            try
            {
                await _postService.UpdateAsync(post);
                _logger.LogInformation("UserId {UserId} successfully updated post {PostId}", userId, model.Id);
                return RedirectToAction("Profile", new { userId = ViewId, viewType = viewType });
            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while updating post {PostId} by UserId {UserId}", model.Id, userId);
                TempData["ErrorPostId"] = model.Id;  // Store post ID for error display
                TempData["ContentErrorMessage"] = "An error occurred while updating the post. Please try again."; // Store error message
                return RedirectToAction("Profile", new { userId = ViewId, viewType = viewType });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id, string viewType, int ViewId)
        {

            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            //Getting Post-object by ID
            var post = await _postService.GetByIdAsync(id);
            //Checking credentials
            if (post == null || post.UserId != userId)
            {
                _logger.LogWarning("Unauthorized attempt to delete post {PostId} by UserId {UserId}", id, userId);
                return Forbid();
            }
            await _postService.DeleteAsync(id);
            //Deleting post
            _logger.LogInformation("UserId {UserId} deleted post {PostId}", userId, id);
            return RedirectToAction("Profile", new { userId = ViewId, viewType = viewType });
        }
    }
}
