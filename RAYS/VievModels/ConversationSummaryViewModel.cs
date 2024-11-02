using System;
using System.Collections.Generic;

namespace RAYS.ViewModels
{
    public class ConversationSummaryViewModel
    {
        public int UserId { get; set; } // ID of the user involved in the conversation
        public string Username { get; set; } = string.Empty; // Username of the other user
        public string LastMessage { get; set; } = string.Empty; // Content of the last message
        public DateTime LastMessageTimestamp { get; set; } // Timestamp of the last message
        public string MessageDirection { get; set; } = string.Empty; // "Sent" or "Received"

        // You can also include other fields as needed
    }
}
