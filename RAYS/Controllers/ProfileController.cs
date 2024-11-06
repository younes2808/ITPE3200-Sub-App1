using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RAYS.Services;
using RAYS.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RAYS.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly PostService _postService;
        private readonly UserService _userService;

        public ProfileController(PostService postService, UserService userService)
        {
            _postService = postService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int userId, string viewType = "Posts")
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.ViewType = viewType; // Set the view type based on the query parameter

            var userViewModel = new UserViewModel
            {
                UserId = user.Id,
                Username = user.Username,
                Posts = await GetPostsForUser(userId),
                LikedPosts = await GetLikedPostsForUser(userId),
                Friends = await GetFriends(userId)
            };

            return View(userViewModel);
        }

        private async Task<List<PostViewModel>> GetPostsForUser(int userId)
        {
            var posts = await _postService.GetByUserIdAsync(userId);
            var postViewModels = new List<PostViewModel>();

            foreach (var post in posts)
            {
                postViewModels.Add(new PostViewModel
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImagePath = post.ImagePath,
                    VideoUrl = post.VideoUrl,
                    Location = post.Location,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    IsLikedByUser = await _postService.IsPostLikedByUserAsync(userId, post.Id)
                });
            }

            return postViewModels;
        }

        private async Task<List<PostViewModel>> GetLikedPostsForUser(int userId)
        {
            var likedPosts = await _postService.GetLikedPostsByUserIdAsync(userId);
            var likedPostViewModels = new List<PostViewModel>();

            foreach (var post in likedPosts)
            {
                likedPostViewModels.Add(new PostViewModel
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImagePath = post.ImagePath,
                    VideoUrl = post.VideoUrl,
                    Location = post.Location,
                    CreatedAt = post.CreatedAt,
                    UserId = post.UserId,
                    IsLikedByUser = true // All liked posts are considered liked
                });
            }

            return likedPostViewModels;
        }

        private async Task<List<UserViewModel>> GetFriends(int userId)
        {
            // Return placeholder friends
            await Task.Delay(50); // Simulate async call

            return new List<UserViewModel>
            {
                new UserViewModel { UserId = 1, Username = "PlaceholderFriend1" },
                new UserViewModel { UserId = 2, Username = "PlaceholderFriend2" },
                new UserViewModel { UserId = 3, Username = "PlaceholderFriend3" }
            };
        }

        [HttpPost]
        public async Task<IActionResult> Like(int postId, string viewType)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (!await _postService.IsPostLikedByUserAsync(userId, postId))
            {
                await _postService.LikePostAsync(userId, postId);
            }

            return RedirectToAction("Profile", new { userId = userId, viewType = viewType });
        }

        [HttpPost]
        public async Task<IActionResult> Unlike(int postId, string viewType)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (await _postService.IsPostLikedByUserAsync(userId, postId))
            {
                await _postService.UnlikePostAsync(userId, postId);
            }

            return RedirectToAction("Profile", new { userId = userId, viewType = viewType });
        }

        [HttpPost]
        [Authorize]  // Ensure only authenticated users can access this
        public async Task<IActionResult> Update(PostViewModel model, string viewType)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (model.UserId != userId)
                return Forbid();

            var post = await _postService.GetByIdAsync(model.Id);
            if (post == null)
                return NotFound();

            post.Content = model.Content;
            post.VideoUrl = model.VideoUrl;
            post.Location = model.Location;

            try
            {
                await _postService.UpdateAsync(post);
                return RedirectToAction("Profile", new { userId = userId, viewType = viewType });
            }
            catch (Exception)
            {
                return View("Index", model); // Handle error feedback as needed
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string viewType)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var post = await _postService.GetByIdAsync(id);
            if (post == null || post.UserId != userId)
                return Forbid();

            await _postService.DeleteAsync(id);
            return RedirectToAction("Profile", new { userId = userId, viewType = viewType });
        }
    }
}
