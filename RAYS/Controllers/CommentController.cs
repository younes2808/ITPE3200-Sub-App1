using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Route("comments")]
    public class CommentController : Controller
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: comments/{postId} (Get all comments for a post)
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetCommentsForPost(int postId)
        {
            var comments = await _commentService.GetCommentsForPost(postId);
            return View(comments); // Return the view with the list of comments
        }

        // GET: comments/add/{postId} (Display the add comment form)
        [HttpGet("add/{postId}")]
        public IActionResult AddComment(int postId)
        {
            ViewBag.PostId = postId; // Pass the postId to the view
            return View(); // Return the view for adding a comment
        }

        // POST: comments/add (Add a comment to a post)
        [HttpPost("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromForm] Comment comment)
        {
            try
            {
                var createdComment = await _commentService.AddComment(comment);
                return RedirectToAction(nameof(GetCommentsForPost), new { postId = comment.PostId }); // Redirect to the comments for that post
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(comment); // Return the view with an error message
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(comment); // Return the view with an error message
            }
        }

        // GET: comments/edit/{id} (Display the edit comment form)
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> EditComment(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id); // Ensure you have a method to get a comment by ID
            if (comment == null)
            {
                return NotFound(); // Return a 404 if the comment is not found
            }
            return View(comment); // Return the view for editing a comment
        }

        // POST: comments/edit (Update a comment)
        [HttpPost("edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment([FromForm] Comment comment)
        {
            try
            {
                var updatedComment = await _commentService.UpdateComment(comment.Id, comment.Text, comment.UserId);
                return RedirectToAction(nameof(GetCommentsForPost), new { postId = comment.PostId }); // Redirect to the comments for that post
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(comment); // Return the view with an error message
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(comment); // Return the view with an error message
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(comment); // Return the view with an error message
            }
        }

        // POST: comments/delete/{id} (Delete a comment)
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id, [FromQuery] int userId)
        {
            try
            {
                await _commentService.DeleteComment(id, userId);
                return RedirectToAction(nameof(GetCommentsForPost), new { postId = id }); // Redirect to the comments for that post
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(GetCommentsForPost), new { postId = id }); // Redirect with an error message
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(GetCommentsForPost), new { postId = id }); // Redirect with an error message
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(GetCommentsForPost), new { postId = id }); // Redirect with an error message
            }
        }
    }
}
