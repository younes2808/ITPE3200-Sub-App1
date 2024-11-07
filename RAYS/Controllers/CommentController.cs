using Microsoft.AspNetCore.Mvc;
using RAYS.Services;
using RAYS.ViewModels;
using RAYS.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace RAYS.Controllers
{
    [Route("[controller]")]
    public class CommentController : Controller
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: /Comment/{postId}
        [HttpGet("{postId}")]
        public async Task<IActionResult> Index(int postId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var comments = await _commentService.GetCommentsForPost(postId);

            // Map comments to the ViewModel and set `IsEditable` for the current user
            var viewModel = comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                PostId = c.PostId,
                UserName = c.UserName,
                IsEditable = c.UserId == userId
            }).ToList();

            ViewBag.PostId = postId;
            return View(viewModel);
        }

        // POST: /Comment/Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add(CommentViewModel commentViewModel)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (commentViewModel.Id == 0)
            {
                // Adding a new comment
                var comment = new Comment
                {
                    Text = commentViewModel.Text,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                    PostId = commentViewModel.PostId
                };

                await _commentService.AddComment(comment);
            }
            else
            {
                // Updating an existing comment
                await _commentService.UpdateComment(commentViewModel.Id, commentViewModel.Text, userId);
            }

            return RedirectToAction("Index", new { postId = commentViewModel.PostId });
        }

        // POST: /Comment/Update
        [HttpPost("Update")]
        public async Task<IActionResult> Update(CommentViewModel commentViewModel)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            try
            {
                await _commentService.UpdateComment(commentViewModel.Id, commentViewModel.Text, userId);
                return RedirectToAction("Index", new { postId = commentViewModel.PostId });
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Index", commentViewModel);
            }
        }

        // POST: /Comment/Delete/{id}
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id, int postId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            try
            {
                await _commentService.DeleteComment(id, userId);
                return RedirectToAction("Index", new { postId });
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Index", new { postId });
            }
        }
    }
}
