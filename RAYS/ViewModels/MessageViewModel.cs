using RAYS.Models;
namespace RAYS.ViewModels
{
    public class MessageViewModel
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        // Ensure that Messages is initialized to an empty list
        public List<Message> Messages { get; set; }

        // Ensure that SenderName and ReceiverName are initialized to empty strings
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public int CurrentUserId { get; set; }

        // Constructor to ensure non-nullable properties are always initialized
        public MessageViewModel()
        {
            Messages = new List<Message>(); // Initialize with an empty list
            SenderName = string.Empty;       // Initialize with an empty string
            ReceiverName = string.Empty;     // Initialize with an empty string
        }
    }
}
