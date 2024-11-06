using System.Collections.Generic;

namespace RAYS.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<PostViewModel> Posts { get; set; } = new();
        public List<PostViewModel> LikedPosts { get; set; } = new();
        public List<UserViewModel> Friends { get; set; } = new();
    }
}
