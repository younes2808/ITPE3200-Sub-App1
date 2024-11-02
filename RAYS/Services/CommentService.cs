using RAYS.Models;
using RAYS.Repositories;
using RAYS.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository; // User repository for fetching user info
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, IUserRepository userRepository, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Comment> AddComment(Comment comment)
        {
            _logger.LogInformation("Attempting to add a new comment.");

            // Check if the post exists
            if (await _commentRepository.GetByIdAsync(comment.PostId) == null)
            {
                _logger.LogWarning("Post with ID {PostId} not found.", comment.PostId);
                throw new KeyNotFoundException("Post not found.");
            }

            // Check if the user exists using GetUserByIdAsync
            if (await _userRepository.GetUserByIdAsync(comment.UserId) == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", comment.UserId);
                throw new KeyNotFoundException("User not found.");
            }

            comment.CreatedAt = DateTime.UtcNow; // Set creation time
            await _commentRepository.AddAsync(comment);

            _logger.LogInformation("Comment added successfully with ID {CommentId}.", comment.Id);
            return comment;
        }

        public async Task<IEnumerable<CommentWithUserViewModel>> GetCommentsForPost(int postId)
        {
            _logger.LogInformation("Retrieving comments for post ID {PostId}.", postId);
            var comments = await _commentRepository.GetAllAsync(postId);
            var userIds = comments.Select(c => c.UserId).Distinct().ToList(); // Get distinct user IDs

            var commentsWithUsernames = new List<CommentWithUserViewModel>();

            foreach (var comment in comments)
            {
                var user = await _userRepository.GetUserByIdAsync(comment.UserId);
                commentsWithUsernames.Add(new CommentWithUserViewModel
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    CreatedAt = comment.CreatedAt,
                    UserId = comment.UserId,
                    PostId = comment.PostId,
                    UserName = user?.Username ?? "Unknown" // Use username or "Unknown" if user not found
                });
            }

            _logger.LogInformation("Retrieved {Count} comments for post ID {PostId}.", commentsWithUsernames.Count, postId);
            return commentsWithUsernames; // Return comments with usernames
        }

        public async Task<Comment> GetCommentByIdAsync(int commentId)
        {
            _logger.LogInformation("Retrieving comment with ID {CommentId}.", commentId);
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                throw new KeyNotFoundException("Comment not found.");
            }
            return comment;
        }

        public async Task<Comment> UpdateComment(int commentId, string text, int userId)
        {
            _logger.LogInformation("Attempting to update comment with ID {CommentId}.", commentId);
            var existingComment = await _commentRepository.GetByIdAsync(commentId);
            if (existingComment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", commentId);
                throw new KeyNotFoundException("Comment not found.");
            }

            if (existingComment.UserId != userId)
            {
                _logger.LogWarning("User with ID {UserId} is not authorized to update comment ID {CommentId}.", userId, commentId);
                throw new UnauthorizedAccessException("User does not have permission to update this comment.");
            }

            existingComment.Text = text;
            await _commentRepository.UpdateAsync(existingComment);

            _logger.LogInformation("Comment with ID {CommentId} updated successfully.", commentId);
            return existingComment;
        }

        public async Task DeleteComment(int id, int userId)
        {
            _logger.LogInformation("Attempting to delete comment with ID {CommentId}.", id);
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found.", id);
                throw new KeyNotFoundException("Comment not found.");
            }

            if (comment.UserId != userId)
            {
                _logger.LogWarning("User with ID {UserId} is not authorized to delete comment ID {CommentId}.", userId, id);
                throw new UnauthorizedAccessException("User does not have permission to delete this comment.");
            }

            await _commentRepository.DeleteAsync(id);
            _logger.LogInformation("Comment with ID {CommentId} deleted successfully.", id);
        }
    }
}
