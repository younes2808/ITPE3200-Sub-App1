namespace RAYS.ViewModels
{
    public class FriendRequestViewModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public required string Status { get; set; }
    }
}
