using Microsoft.EntityFrameworkCore;
using RAYS.DAL;
using RAYS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly ServerAPIContext _context;

        public FriendRepository(ServerAPIContext context)
        {
            _context = context;
        }

        public async Task<bool> SendFriendRequestAsync(Friend friend)
        {
            // Check if sender and receiver are the same
            if (friend.SenderId == friend.ReceiverId)
                return false; // Sender and receiver cannot be the same

            // Check if an existing friend request already exists
            var existingRequest = await _context.Friends
                .FirstOrDefaultAsync(f => 
                    (f.SenderId == friend.SenderId && f.ReceiverId == friend.ReceiverId) ||
                    (f.SenderId == friend.ReceiverId && f.ReceiverId == friend.SenderId));

            if (existingRequest != null)
                return false; // A friend request already exists

            // Set status to "Pending"
            friend.Status = "Pending";

            // Add new friend request to the database
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();
            return true; // Friend request sent
        }

        public async Task<IEnumerable<Friend>> GetFriendRequestsAsync(int userId)
        {
            // Retrieve friend requests for the user
            return await _context.Friends
                .Where(f => (f.ReceiverId == userId || f.SenderId == userId) && f.Status == "Pending")
                .ToListAsync();
        }

        public async Task<Friend> GetFriendRequestByIdAsync(int id)
        {
            // Retrieve friend request by its ID
            var friendRequest = await _context.Friends.FindAsync(id);
            
            if (friendRequest == null)
            {
                throw new KeyNotFoundException($"Friend request with ID {id} not found.");
            }

            return friendRequest;
        }

        public async Task<bool> AcceptFriendRequestAsync(int id)
        {
            var request = await GetFriendRequestByIdAsync(id);
            if (request == null) return false; // Request not found

            // Update status to "Accepted"
            request.Status = "Accepted";
            await _context.SaveChangesAsync();
            return true; // Friend request accepted
        }

        public async Task<bool> RejectFriendRequestAsync(int id)
        {
            var request = await GetFriendRequestByIdAsync(id);
            if (request == null) return false; // Request not found

            // Remove the friend request from the database
            _context.Friends.Remove(request);
            await _context.SaveChangesAsync();
            return true; // Friend request rejected
        }

        public async Task<IEnumerable<Friend>> GetFriendsAsync(int userId)
        {
            // Retrieve all friends for the user
            return await _context.Friends
                .Where(f => (f.SenderId == userId || f.ReceiverId == userId) && f.Status == "Accepted")
                .ToListAsync();
        }

        public async Task<bool> DeleteFriendAsync(int userId, int friendId)
        {
            var friendRelationship = await _context.Friends
                .FirstOrDefaultAsync(f => 
                    (f.SenderId == userId && f.ReceiverId == friendId) ||
                    (f.SenderId == friendId && f.ReceiverId == userId) &&
                    f.Status == "Accepted");

            if (friendRelationship == null) return false; // Friendship not found

            // Remove the friendship from the database
            _context.Friends.Remove(friendRelationship);
            await _context.SaveChangesAsync();
            return true; // Friend deleted
        }
    }
}
