using System.ComponentModel.DataAnnotations;

namespace RAYS.Models
{
    public class MessageRequest
    {
        [Required(ErrorMessage = "Sender ID is required.")]
        public int SenderId { get; set; } // Foreign key: Sender of the message

        [Required(ErrorMessage = "Receiver ID is required.")]
        public int ReceiverId { get; set; } // Foreign key: Receiver of the message

        [Required(ErrorMessage = "Message content is required.")]
        [StringLength(1000, ErrorMessage = "Message content cannot exceed 1000 characters.")]
        public required string Content { get; set; } // The content of the message
    }
}
