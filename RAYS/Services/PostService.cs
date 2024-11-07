using RAYS.Models;
using RAYS.Repositories;
using System.IO;
using System.Threading.Tasks;

namespace RAYS.Services
{
    public class PostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _postRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Post>> GetLatestPostsAsync(int count)
        {
            return await _postRepository.GetLatestPostsAsync(count);
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
        {
            return await _postRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Post>> GetLikedPostsByUserIdAsync(int userId)
        {
            return await _postRepository.GetLikedPostsByUserIdAsync(userId);
        }

        public async Task AddAsync(Post post)
        {
            await _postRepository.AddAsync(post);
        }

        public async Task UpdateAsync(Post post)
        {
            await _postRepository.UpdateAsync(post);
        }

        public async Task DeleteAsync(int id)
        {
            await _postRepository.DeleteAsync(id);
        }

        public async Task LikePostAsync(int userId, int postId)
        {
            var like = new Like { UserId = userId, PostId = postId };
            await _postRepository.AddLikeAsync(like);
        }

        public async Task UnlikePostAsync(int userId, int postId)
        {
            await _postRepository.RemoveLikeAsync(userId, postId);
        }

        public async Task<bool> IsPostLikedByUserAsync(int userId, int postId)
        {
            return await _postRepository.HasUserLikedPostAsync(userId, postId);
        }

        // Move SaveImage logic here
        public async Task<string?> SaveImageAsync(IFormFile? image)
        {
            if (image == null || image.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/images/{uniqueFileName}";
        }
    }
}
