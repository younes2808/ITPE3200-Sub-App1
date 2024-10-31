using Microsoft.AspNetCore.Mvc;
using RAYS.Models;
using RAYS.Services;
using System;
using System.Threading.Tasks;

namespace RAYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        // POST: api/Comment (Legg til en kommentar til en post)
        [HttpPost]
        public async Task<ActionResult<Comment>> AddComment([FromBody] Comment comment)
        {
            try
            {
                var createdComment = await _commentService.AddComment(comment);
                return Ok(createdComment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Comment/{postId} (Hent alle kommentarer for en post)
        [HttpGet("{postId}")]
        public async Task<ActionResult> GetCommentsForPost(int postId)
        {
            var comments = await _commentService.GetCommentsForPost(postId);
            return Ok(comments);
        }

        // PUT: api/Comment/update
        [HttpPut("update")]
        public async Task<ActionResult<Comment>> UpdateComment([FromBody] UpdateCommentRequest request)
        {
            try
            {
                var updatedComment = await _commentService.UpdateComment(request.CommentId, request.Text, request.UserId);
                return Ok(updatedComment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Comment/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment(int id, [FromQuery] int userId)
        {
            try
            {
                await _commentService.DeleteComment(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
