namespace RAYS.ViewModels
{
    public class FriendRequestViewModel
    {
        public int Id { get; set; } // ID for the friend request
        public int SenderId { get; set; } // ID of the sender
        public string? SenderUsername { get; set; } // Username of the sender
        public int ReceiverId { get; set; } // ID of the receiver
        public string? ReceiverUsername { get; set; } // Username of the receiver (optional)
        public string? Status { get; set; } // Status of the friend request (Pending, Accepted, Rejected)
    }
}
