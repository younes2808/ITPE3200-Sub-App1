// In RAYS/ViewModels/ConversationsViewModel.cs
namespace RAYS.ViewModels
{
    public class ConversationsViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }  // Add this property for username
        public string LastMessage { get; set; }
        public DateTime LastMessageTimestamp { get; set; }
        public bool IsResponded { get; set; } // Add this property to track if the conversation has been responded to
        public bool HasNewMessages { get; set; } // Optional: to track if there are unread messages
    }

}
