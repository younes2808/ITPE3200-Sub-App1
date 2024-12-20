using Microsoft.Extensions.Logging;
using RAYS.Models;
using RAYS.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAYS.Services
{
    public class FriendService
    {
        private readonly IFriendRepository _friendRepository;
        private readonly ILogger<FriendService> _logger;

        public FriendService(IFriendRepository friendRepository, ILogger<FriendService> logger)
        {
            _friendRepository = friendRepository;
            _logger = logger;
        }

        public async Task<bool> SendFriendRequestAsync(Friend friend)
        {
            if (!friend.IsValid())
            {
                _logger.LogWarning($"Invalid friend request from UserId: {friend.SenderId} to UserId: {friend.ReceiverId}.");
                return false; // Return false if the request is invalid
            }

            var result = await _friendRepository.SendFriendRequestAsync(friend);
            if (result)
            {
                _logger.LogInformation($"Friend request sent from UserId: {friend.SenderId} to UserId: {friend.ReceiverId}.");
            }
            else
            {
                _logger.LogWarning($"Failed to send friend request from UserId: {friend.SenderId} to UserId: {friend.ReceiverId}.");
            }
            return result;
        }

        public async Task<IEnumerable<Friend>> GetFriendRequestsAsync(int userId)
        {
            var requests = await _friendRepository.GetFriendRequestsAsync(userId);
            return requests;
        }

        public async Task<bool> AcceptFriendRequestAsync(int id)
        {
            var result = await _friendRepository.AcceptFriendRequestAsync(id);
            if (result)
            {
                _logger.LogInformation($"Friend request {id} accepted.");
            }
            else
            {
                _logger.LogWarning($"Friend request {id} not found for acceptance.");
            }
            return result;
        }

        public async Task<bool> RejectFriendRequestAsync(int id)
        {
            var result = await _friendRepository.RejectFriendRequestAsync(id);
            if (result)
            {
                _logger.LogInformation($"Friend request {id} rejected.");
            }
            else
            {
                _logger.LogWarning($"Friend request {id} not found for rejection.");
            }
            return result;
        }

        public async Task<IEnumerable<Friend>> GetFriendsAsync(int userId)
        {
            var friends = await _friendRepository.GetFriendsAsync(userId);
            //_logger.LogInformation($"Retrieved {requests.Count()} friend requests for user {userId}.");
            // Dont want to use Logger here because this gets invoked often
            return friends;
        }

        public async Task<bool> DeleteFriendAsync(int userId, int friendId)
        {
            var result = await _friendRepository.DeleteFriendAsync(userId, friendId);
            if (result)
            {
                _logger.LogInformation("User with ID {CurrentUserId} has deleted their friend with ID {FriendId}.", userId, friendId);

            }
            else
            {
                _logger.LogWarning($"Friend {friendId} not found for user {userId}.");
            }
            return result;
        }
    }
}
