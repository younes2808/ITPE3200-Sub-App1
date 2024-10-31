using RAYS.DAL;
using RAYS.Models;
using Microsoft.EntityFrameworkCore; // Importer denne linjen for ToListAsync()
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Services
{
    public class CommentService
    {
        private readonly ServerAPIContext _context;

        public CommentService(ServerAPIContext context)
        {
            _context = context;
        }

        public async Task<Comment> AddComment(Comment comment)
        {
            // Sjekk om posten finnes
            if (!_context.Posts.Any(p => p.Id == comment.PostId))
            {
                throw new KeyNotFoundException("Post not found.");
            }

            // Sjekk om brukeren finnes
            if (!_context.Users.Any(u => u.Id == comment.UserId))
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Legg til kommentaren
            comment.CreatedAt = DateTime.UtcNow; // Sett CreatedAt
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment; // Returner den opprettede kommentaren
        }

        public async Task<List<Comment>> GetCommentsForPost(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(); // Bruker ToListAsync() fra EF Core
        }

        public async Task<Comment> UpdateComment(int commentId, string text, int userId)
        {
            var existingComment = await _context.Comments.FindAsync(commentId);
            if (existingComment == null)
            {
                throw new KeyNotFoundException("Comment not found.");
            }

            // Sjekk om bruker ID samsvarer
            if (existingComment.UserId != userId)
            {
                throw new UnauthorizedAccessException("User does not have permission to update this comment.");
            }

            // Oppdater kommentaren
            existingComment.Text = text;
            await _context.SaveChangesAsync();

            return existingComment; // Returner den oppdaterte kommentaren
        }

        public async Task DeleteComment(int id, int userId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                throw new KeyNotFoundException("Comment not found.");
            }

            // Sjekk om bruker ID samsvarer
            if (comment.UserId != userId)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete this comment.");
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
