using System.Collections.Generic;
using System.Threading.Tasks;
using RAYS.Models;

namespace RAYS.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(int id); // Change this to Comment?
        Task<IEnumerable<Comment>> GetAllAsync(int postId);
        Task AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
        Task<IEnumerable<Comment>> GetByUserIdAsync(int userId);
    }
}
