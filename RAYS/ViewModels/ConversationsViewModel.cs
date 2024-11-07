namespace RAYS.ViewModels
{
    public class ConversationsViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTimestamp { get; set; }
        public bool IsResponded { get; set; }
        public bool HasNewMessages { get; set; }

        // Constructor to ensure non-null values
        public ConversationsViewModel(int userId, string username, string lastMessage, DateTime lastMessageTimestamp, bool isResponded, bool hasNewMessages)
        {
            UserId = userId;
            Username = username ?? throw new ArgumentNullException(nameof(username)); // Ensure it's not null
            LastMessage = lastMessage ?? throw new ArgumentNullException(nameof(lastMessage)); // Ensure it's not null
            LastMessageTimestamp = lastMessageTimestamp;
            IsResponded = isResponded;
            HasNewMessages = hasNewMessages;
        }
    }
}
