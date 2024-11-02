namespace RAYS.ViewModels
{
    public class FriendListViewModel
    {
        public List<FriendViewModel> Friends { get; set; } = new List<FriendViewModel>();
    }

    public class FriendViewModel
    {
        public int FriendId { get; set; }
        public required string FriendName { get; set; } // Assume you have a way to get a friend's name.
    }
}
