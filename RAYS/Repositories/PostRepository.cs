using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RAYS.DAL;
using RAYS.Models;

namespace RAYS.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ServerAPIContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(ServerAPIContext context, IWebHostEnvironment webHostEnvironment, ILogger<PostRepository> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts.FindAsync(id);
        }

        public async Task<IEnumerable<Post>> GetLatestPostsAsync(int count)
        {
            return await _context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetLikedPostsByUserIdAsync(int userId)
        {
            var likedPostIds = await _context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.PostId)
                .ToListAsync();

            return await _context.Posts
                .Where(p => likedPostIds.Contains(p.Id))
                .ToListAsync();
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
    {
        var post = await GetByIdAsync(id);
        if (post != null)
        {
            if (!string.IsNullOrEmpty(post.ImagePath))
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            // Check if post.ImagePath contains "/images/" and remove it
            var relativeImagePath = post.ImagePath.Replace("/images/", "");
            var filePath = Path.Combine(uploadsFolder, relativeImagePath);

            _logger.LogInformation("Full file path for deletion:");
            _logger.LogInformation(filePath);

            // Check if the file exists before trying to delete
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                _logger.LogInformation("Image file deleted successfully.");
            }
            else
            {
                _logger.LogWarning("Image file not found at path: " + filePath);
            }
        }

        // Now delete the post from the database
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        }
    }


        // Method to like a post
        public async Task AddLikeAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        // Method to check if a user has liked a post
        public async Task<bool> HasUserLikedPostAsync(int userId, int postId)
        {
            return await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId);
        }

        // Method to remove a like
        public async Task RemoveLikeAsync(int userId, int postId)
        {
            var like = await _context.Likes.FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }
        
    }
}
