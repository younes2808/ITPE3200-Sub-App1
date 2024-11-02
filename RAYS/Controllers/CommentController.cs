using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using RAYS.ViewModels;
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
            var model = new CommentsPageViewModel
            {
                PostId = postId,
                Comments = comments // Now includes usernames
            };

            return View(model); // Return the view with comments
        }

        // ... other methods remain unchanged
    }
}
