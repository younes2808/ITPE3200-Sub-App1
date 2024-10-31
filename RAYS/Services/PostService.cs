using RAYS.Models;
using RAYS.Repositories;
using System.Collections.Generic;
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
    }
}
