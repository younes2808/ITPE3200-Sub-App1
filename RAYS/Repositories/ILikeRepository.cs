using RAYS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public interface ILikeRepository
    {
        Task<Like?> GetLikeAsync(int userId, int postId);
        Task AddLikeAsync(Like like);
        Task RemoveLikeAsync(Like like);
        Task<IEnumerable<Like>> GetLikesForPostAsync(int postId);
    }
}
