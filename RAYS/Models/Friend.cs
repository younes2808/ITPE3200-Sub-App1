using System.ComponentModel.DataAnnotations;

namespace RAYS.Models
{
    public class Friend
    {
        public int Id { get; set; }

        // Foreign key: Sender of the friend request
        [Required]
        public int SenderId { get; set; }

        // Foreign key: Receiver of the friend request
        [Required]
        public int ReceiverId { get; set; }

        // Friend request status: Pending, Accepted, Rejected
        [Required]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; } = "Pending";

        // Ensure that Sender and Receiver can't be the same user
        public bool IsValid()
        {
            return SenderId != ReceiverId;
        }
    }
}
