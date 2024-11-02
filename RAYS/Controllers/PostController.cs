using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using RAYS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Route("posts")]
    public class PostController : Controller
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        // GET: posts
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetLatestPostsAsync(10); // Example: Get the latest 10 posts
            return View(posts); // Return the view with the posts
        }

        // GET: posts/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(); // Return the create view
        }

        // POST: posts/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return the view with validation errors
            }

            var post = new Post
            {
                Content = model.Content,
                ImagePath = model.Image != null ? await SaveImage(model.Image) : null,
                VideoUrl = model.VideoUrl,
                Location = model.Location,
                UserId = model.UserId
            };

            await _postService.AddAsync(post);
            return RedirectToAction(nameof(Index)); // Redirect to the list of posts
        }

        // GET: posts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound(); // Return a 404 if the post is not found
            }
            return View(post); // Return the details view with the post
        }

        // GET: posts/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound(); // Return a 404 if the post is not found
            }

            var model = new PostViewModel
            {
                Id = post.Id,
                Content = post.Content,
                VideoUrl = post.VideoUrl,
                Location = post.Location,
                UserId = post.UserId
                // ImagePath is typically not set here, as it's not user input
            };

            return View(model); // Return the edit view with the post
        }

        // POST: posts/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] PostViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest(); // Return bad request if IDs do not match
            }

            if (!ModelState.IsValid)
            {
                return View(model); // Return the view with validation errors
            }

            var post = new Post
            {
                Id = model.Id,
                Content = model.Content,
                ImagePath = model.Image != null ? await SaveImage(model.Image) : null,
                VideoUrl = model.VideoUrl,
                Location = model.Location,
                UserId = model.UserId
            };

            await _postService.UpdateAsync(post);
            return RedirectToAction(nameof(Index)); // Redirect to the list of posts
        }

        // POST: posts/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _postService.DeleteAsync(id);
            return RedirectToAction(nameof(Index)); // Redirect to the list of posts
        }

        private async Task<string?> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0) return null;

            // Define the path to save the image
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Ensure the directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Save the file asynchronously
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/images/{uniqueFileName}"; // Return the relative path to access the image
        }
    }
}
