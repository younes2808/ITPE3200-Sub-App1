using System.Collections.Generic;

namespace RAYS.Models
{
    public class MessagesViewModel
    {
        public IEnumerable<Message> Messages { get; set; } = new List<Message>();
        
        public MessageRequest MessageRequest { get; set; } = new MessageRequest
        {
            SenderId = 0, // Default value, can be set when the ViewModel is created in the controller
            ReceiverId = 0, // Default value, can be set when the ViewModel is created in the controller
            Content = string.Empty // Ensure Content is initialized
        };
    }
}
