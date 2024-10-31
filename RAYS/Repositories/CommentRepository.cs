using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RAYS.DAL;
using RAYS.Models;

namespace RAYS.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ServerAPIContext _context;

        public CommentRepository(ServerAPIContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetAllAsync(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var comment = await GetByIdAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> GetByUserIdAsync(int userId)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }
    }
}
