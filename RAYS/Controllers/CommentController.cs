using Microsoft.AspNetCore.Mvc;
using RAYS.Services;
using RAYS.ViewModels;
using RAYS.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace RAYS.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CommentController : Controller
    {
        private readonly CommentService _commentService;
        private readonly ILogger<CommentController> _logger; // Added for logging

        public CommentController(CommentService commentService, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        // GET: /Comment/{postId}
        [HttpGet("{postId}")]
        public async Task<IActionResult> Index(int postId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            try
            {
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
            catch (KeyNotFoundException ex)
            {
                // Handle not found exceptions gracefully
                _logger.LogWarning(ex, "Post with ID {PostId} not found.", postId);
                TempData["ErrorMessage"] = "Post not found.";
                return RedirectToAction("Index", new { postId });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                _logger.LogError(ex, "An unexpected error occurred while retrieving comments.");
                TempData["ErrorMessage"] = "An error occurred while retrieving comments.";
                return RedirectToAction("Index", new { postId });
            }
        }

        // POST: /Comment/Add
        [HttpPost("Add")]
        public async Task<IActionResult> Add(CommentViewModel commentViewModel)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (string.IsNullOrEmpty(commentViewModel.Text))
            {
                TempData["ErrorMessage"] = "Comment text cannot be empty.";
                return RedirectToAction("Index", new { postId = commentViewModel.PostId });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
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
            catch (KeyNotFoundException ex)
            {
                // Handle case where post or user is not found
                _logger.LogWarning(ex, "Error occurred while adding/updating comment.");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { postId = commentViewModel.PostId });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access cases
                _logger.LogWarning(ex, "User not authorized to modify comment.");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { postId = commentViewModel.PostId });
            }
            catch (Exception ex)
            {
                // General exception handling
                _logger.LogError(ex, "An unexpected error occurred while adding/updating the comment.");
                TempData["ErrorMessage"] = "An error occurred while processing the comment.";
                return RedirectToAction("Index", new { postId = commentViewModel.PostId });
            }
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
                // Handle cases where the comment is not found
                _logger.LogWarning(ex, "Comment not found with ID {CommentId}.", commentViewModel.Id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { postId = commentViewModel.PostId });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access cases
                _logger.LogWarning(ex, "User not authorized to update comment ID {CommentId}.", commentViewModel.Id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { postId = commentViewModel.PostId });
            }
            catch (Exception ex)
            {
                // General exception handling
                _logger.LogError(ex, "An unexpected error occurred while updating the comment.");
                TempData["ErrorMessage"] = "An error occurred while updating the comment.";
                return RedirectToAction("Index", new { postId = commentViewModel.PostId });
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
                // Handle cases where the comment is not found
                _logger.LogWarning(ex, "Comment not found with ID {CommentId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { postId });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle unauthorized access cases
                _logger.LogWarning(ex, "User not authorized to delete comment ID {CommentId}.", id);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new { postId });
            }
            catch (Exception ex)
            {
                // General exception handling
                _logger.LogError(ex, "An unexpected error occurred while deleting the comment.");
                TempData["ErrorMessage"] = "An error occurred while deleting the comment.";
                return RedirectToAction("Index", new { postId });
            }
        }
    }
}
