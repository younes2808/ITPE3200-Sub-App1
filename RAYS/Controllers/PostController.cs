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
            var postViewModels = posts.Select(post => new PostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                ImagePath = post.ImagePath,
                VideoUrl = post.VideoUrl,
                Location = post.Location,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId,
                IsLikedByUser = _postService.IsPostLikedByUserAsync(userId, post.Id).Result
            }).ToList();

            return View(postViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel model, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            
            // Save the uploaded image if it exists
            var imagePath = await SaveImage(image);

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

        //method for saving photos
        private async Task<string?> SaveImage(IFormFile? image)
        {
            if (image == null || image.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/images/{uniqueFileName}";
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
