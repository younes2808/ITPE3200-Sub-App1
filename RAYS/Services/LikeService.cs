using Microsoft.Extensions.Logging;
using RAYS.Models;
using RAYS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Services
{
    public class LikeService
    {
        private readonly ILikeRepository _likeRepository;
        private readonly ILogger<LikeService> _logger;

        public LikeService(ILikeRepository likeRepository, ILogger<LikeService> logger)
        {
            _likeRepository = likeRepository;
            _logger = logger;
        }

        public async Task<bool> LikePostAsync(Like like)
        {
            var existingLike = await _likeRepository.GetLikeAsync(like.UserId, like.PostId);
            if (existingLike != null)
            {
                _logger.LogWarning("Post already liked by user: {UserId} on Post: {PostId}", like.UserId, like.PostId);
                return false; // Returnerer false hvis liken allerede eksisterer
            }

            await _likeRepository.AddLikeAsync(like);
            _logger.LogInformation("Post liked successfully by user: {UserId} on Post: {PostId}", like.UserId, like.PostId);
            return true; // Returnerer true hvis liken ble lagt til
        }

        public async Task<bool> UnlikePostAsync(Like like)
        {
            var existingLike = await _likeRepository.GetLikeAsync(like.UserId, like.PostId);
            if (existingLike == null)
            {
                _logger.LogWarning("Like not found for user: {UserId} on Post: {PostId}", like.UserId, like.PostId);
                return false; // Returnerer false hvis liken ikke eksisterer
            }

            await _likeRepository.RemoveLikeAsync(existingLike);
            _logger.LogInformation("Post unliked successfully by user: {UserId} on Post: {PostId}", like.UserId, like.PostId);
            return true; // Returnerer true hvis liken ble fjernet
        }

        public async Task<IEnumerable<Like>> GetLikesForPostAsync(int postId)
        {
            return await _likeRepository.GetLikesForPostAsync(postId);
        }
    }
}
