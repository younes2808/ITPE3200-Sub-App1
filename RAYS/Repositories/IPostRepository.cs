using RAYS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(int id);
        Task<IEnumerable<Post>> GetLatestPostsAsync(int count);
        Task<IEnumerable<Post>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Post>> GetLikedPostsByUserIdAsync(int userId);
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(int id);

        // New methods for likes
        Task AddLikeAsync(Like like);
        Task<bool> HasUserLikedPostAsync(int userId, int postId);
        Task RemoveLikeAsync(int userId, int postId);
    }
}
