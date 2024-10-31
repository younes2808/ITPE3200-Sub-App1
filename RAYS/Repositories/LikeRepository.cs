using Microsoft.EntityFrameworkCore;
using RAYS.Models;
using RAYS.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ServerAPIContext _context; // Erstatt med navnet på din DbContext

        public LikeRepository(ServerAPIContext context)
        {
            _context = context;
        }

        public async Task<Like?> GetLikeAsync(int userId, int postId)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        }

        public async Task AddLikeAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLikeAsync(Like like)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Like>> GetLikesForPostAsync(int postId)
        {
            return await _context.Likes
                .Where(l => l.PostId == postId)
                .ToListAsync();
        }
    }
}
