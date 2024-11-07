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
        private readonly ILogger<PostController> _logger;


        public PostController(PostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
             _logger = logger;
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
            // Check if content is empty or null
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                TempData["ErrorMessage"] = "Please enter some text before posting.";
                return RedirectToAction("Index");
            }

            // Check if ModelState is valid before any other checks
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            // Check if an image file was provided
            if (image != null && image.Length > 0)
            {
                // Define a list of allowed image MIME types
                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

                // Check if the uploaded file is an image based on MIME type
                if (!allowedMimeTypes.Contains(image.ContentType))
                {
                    // Store the error message in TempData
                    TempData["ErrorMessage"] = "Please upload a valid image (JPEG, PNG, GIF, BMP, WebP).";

                    // Log the invalid file type for debugging purposes
                    _logger.LogWarning("Invalid file type uploaded. MIME Type: {MimeType}, FileName: {FileName}", image.ContentType, image.FileName);

                    // Redirect back to the page with the error message
                    return RedirectToAction("Index");
                }

                // Optional: Check if the file extension matches the allowed image extensions (as a secondary check)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var fileExtension = Path.GetExtension(image.FileName)?.ToLower();
                if (fileExtension == null || !allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Please upload a valid image file.";
                    return RedirectToAction("Index");
                }
            }

            // If the content is valid (not empty), continue with saving and processing the post
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            try
            {
                // Save the uploaded image using the service method, if an image was provided
                var imagePath = image != null ? await _postService.SaveImageAsync(image) : null;

                // Create and save the post object
                var post = new Post
                {
                    Content = model.Content,
                    ImagePath = imagePath, // If no image, it will be null
                    VideoUrl = model.VideoUrl,
                    Location = model.Location,
                    CreatedAt = DateTime.Now,
                    UserId = userId
                };

                await _postService.AddAsync(post);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception and show a general error message
                _logger.LogError(ex, "An error occurred while saving the image or creating the post.");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(PostViewModel model)
        {
            // Get the current logged-in user's ID
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (model.UserId != userId)
                return Forbid();

            // Check if the content is empty
            if (string.IsNullOrEmpty(model.Content))
            {
                // Store the postId for the post being updated in TempData to show error on the correct post
                TempData["ErrorPostId"] = model.Id;
                _logger.LogInformation("iden til post med feil: ");
                _logger.LogInformation(model.Id.ToString());
                // Store content-specific error message in TempData
                TempData["ContentErrorMessage"] = "Updated Content cannot be empty";

                // Return to the same page with the posts
                return RedirectToAction("Index");
            }

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
                // Store the postId and error message in TempData
                TempData["ErrorPostId"] = model.Id;
                TempData["ContentErrorMessage"] = "An error occurred while updating the post. Please try again.";

                // Return to the same page with the posts
                return RedirectToAction("Index");
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
