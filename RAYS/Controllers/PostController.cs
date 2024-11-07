using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RAYS.Services;
using RAYS.ViewModels;
using RAYS.Models;

namespace RAYS.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var posts = await _postService.GetLatestPostsAsync(20);
            
            // Collect all the tasks to check if the post is liked by the user
            var postViewModels = await Task.WhenAll(posts.Select(async post => new PostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                ImagePath = post.ImagePath,
                VideoUrl = post.VideoUrl,
                Location = post.Location,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                IsLikedByUser = await _postService.IsPostLikedByUserAsync(userId, post.Id)
            }));

            return View(postViewModels);
        }


        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel model, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
            // Save the uploaded image using the service method
            var imagePath = await _postService.SaveImageAsync(image);

            var post = new Post
            {
                Content = model.Content,
                ImagePath = imagePath,
                VideoUrl = model.VideoUrl,
                Location = model.Location,
                CreatedAt = DateTime.Now,
                UserId = userId
            };

            await _postService.AddAsync(post);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]  // Ensure only authenticated users can access this
        public async Task<IActionResult> Update(PostViewModel model)
        {
            // Get the current logged-in user's ID
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (model.UserId != userId)
                return Forbid();

            // Retrieve the post to be updated
            var post = await _postService.GetByIdAsync(model.Id);
            if (post == null)
                return NotFound();

            // Update fields (excluding ImagePath)
            post.Content = model.Content;
            post.VideoUrl = model.VideoUrl;
            post.Location = model.Location;

            try
            {
                await _postService.UpdateAsync(post);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View("Index", model); // Return to Index with error feedback
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            var post = await _postService.GetByIdAsync(id);
            if (post == null || post.UserId != userId)
                return Forbid();

            await _postService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Like(int postId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (!await _postService.IsPostLikedByUserAsync(userId, postId))
                await _postService.LikePostAsync(userId, postId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Unlike(int postId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (await _postService.IsPostLikedByUserAsync(userId, postId))
                await _postService.UnlikePostAsync(userId, postId);

            return RedirectToAction("Index");
        }
    }
}
