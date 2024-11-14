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
        private readonly IPostRepository _postRepository;  // Added PostRepository for post checks
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository, ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Comment> AddComment(Comment comment)
        {

            // Check if the post exists
            var post = await _postRepository.GetByIdAsync(comment.PostId);
            if (post == null)
            {
                _logger.LogWarning("Post with ID {PostId} not found.", comment.PostId);
                throw new KeyNotFoundException("Post not found.");
            }

            // Check if the user exists
            if (await _userRepository.GetUserByIdAsync(comment.UserId) == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", comment.UserId);
                throw new KeyNotFoundException("User not found.");
            }

            comment.CreatedAt = DateTime.UtcNow;
            await _commentRepository.AddAsync(comment);

            _logger.LogInformation("Comment added successfully with ID {CommentId}.", comment.Id);
            return comment;
        }

        public async Task<IEnumerable<CommentViewModel>> GetCommentsForPost(int postId)
        {
            // Logging the request
            // This has many instances so we will not be using it
            // This was mainly for debugging purposes
            //_logger.LogInformation("Retrieving comments for post ID {PostId}.", postId);

            // Check if the post exists
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) 
            {
                _logger.LogInformation("Post not found");
                // If the post does not exist, throw a KeyNotFoundException
                throw new KeyNotFoundException($"Post with ID {postId} not found.");
            }

            // If the post exists, retrieve the comments
            var comments = await _commentRepository.GetAllAsync(postId);
            var commentsWithUsernames = new List<CommentViewModel>();

            // Process each comment
            foreach (var comment in comments)
            {
                var user = await _userRepository.GetUserByIdAsync(comment.UserId);
                commentsWithUsernames.Add(new CommentViewModel
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    CreatedAt = comment.CreatedAt,
                    UserId = comment.UserId,
                    PostId = comment.PostId,
                    UserName = user?.Username ?? "Unknown" // Handle null user
                });
            }

            // Log the number of comments retrieved
            //NOT IN USE DUE TO MANY INSTANCES BEING LOGGED
            //_logger.LogInformation("Retrieved {Count} comments for post ID {PostId}.", commentsWithUsernames.Count, postId);

            // Return the list of comments with usernames
            return commentsWithUsernames;
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
