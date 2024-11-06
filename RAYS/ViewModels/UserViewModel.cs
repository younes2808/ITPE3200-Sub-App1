using System.Collections.Generic;
using RAYS.Models;

namespace RAYS.ViewModels
{
    public class UserViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<PostViewModel> Posts { get; set; } = new();
    public List<PostViewModel> LikedPosts { get; set; } = new();
    public List<UserViewModel> Friends { get; set; } = new();
    public IEnumerable<Friend> FriendRequests { get; set; } = new List<Friend>();
    
    // New properties to track friend request and friendship status
    public bool IsFriend { get; set; }
    public bool HasPendingRequest { get; set; }
}
}
